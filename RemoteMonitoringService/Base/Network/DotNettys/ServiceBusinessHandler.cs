using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Mediator.Net;
using Newtonsoft.Json;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoring.Core.Services.Networks.Base;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;
using RemoteMonitoring.Core.Services.Networks.Base.Messages;
using RemoteMonitoring.Core.Services.Networks.Base.SocketPackets;
using RemoteMonitoring.Core.Utils;
using RemoteMonitoringService.Base.MessageBusModels;
using RemoteMonitoringService.Base.Network.Handlers;
using RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

namespace RemoteMonitoringService.Base.Network.DotNettys;

[AsType(LifetimeEnum.SingleInstance)]
public partial class ServiceBusinessHandler(
    IMediator mediator,
    IServiceNetworkService networkService,
    ContentPanelViewModel contentPanelViewModel,
    ChannelCloseSwitch channelCloseSwitch) : SimpleChannelInboundHandler<NetworkVerify>
{
    private Guid ServerMachineId { get; set; }

    private Guid LinkMachineId { get; set; }

    private SendLogBusModel SendLogBusModel { get; set; } = new();

    protected override async void ChannelRead0(IChannelHandlerContext ctx, NetworkVerify msg)
    {
        try
        {
            var msgHeader = msg.PacketHeader;
            var payload = msg.ReceiveMessageBytes;
            var verifyNetworkMessageResponse =
                mediator.SendAsync<VerifyNetworkMessageCommand, VerifyNetworkMessageResponse>(
                    new VerifyNetworkMessageCommand
                    {
                        Msg = msg
                    }).Result;
            //收到空数据包直接return
            if (!verifyNetworkMessageResponse.IsVerify) return;

            var linkChannel = new ClientLinkChannel(msg.PacketHeader.LinkType, ctx.Channel);
            if (HandleHeartbeatPackets(msg, msgHeader, linkChannel)) return;

            var message = Encoding.UTF8.GetString(payload);
            // 消息来自控制台则发给客户端, 否则则转发给客户端
            switch (msgHeader.LinkType)
            {
                case MachineLinkType.Console:
                {
                    var networkCommand = JsonConvert.DeserializeObject<NetworkCommand>(message);
                    ExecuteNetworkCommand(linkChannel, networkCommand, payload);
                    break;
                }
                case MachineLinkType.Client:
                    var networkResponse = JsonConvert.DeserializeObject<NetworkResponse>(message);
                    await ForwardNetworkResponse(networkResponse, payload, linkChannel);
                    break;
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
    }

    [Description("心跳处理")]
    public override void UserEventTriggered(IChannelHandlerContext ctx, object evt)
    {
        var machineKey = ctx.Channel.GetAttribute(ChannelAttributes.MachineKey).Get();
        if (machineKey != null)
        {
            if (evt is IdleStateEvent idleEvent)
            {
                switch (idleEvent.State)
                {
                    case IdleState.ReaderIdle:
                        if (networkService.AllChannels.TryGetValue(machineKey.MachineId, out var value))
                        {
                            channelCloseSwitch.ModifyChannelCloseState(machineKey.MachineId);
                            value.Dispose();
                        }
                        break;
                    case IdleState.WriterIdle:
                        if (networkService.AllChannels.TryGetValue(machineKey.MachineId, out _))
                        {
                            var heartbeatMessageJson = NetworkHeartbeat.GenerateJson(ServerMachineId);
                            var heartbeatMessageBytes = Encoding.UTF8.GetBytes(heartbeatMessageJson);
                            var timestamp = PacketHeader.ConvertToUnixTimestamp();
                            var header = PacketHeader.Create(
                                PacketHeader.VersionConst, MessageType.Response, heartbeatMessageBytes.Length,
                                PacketHeader.GenerateChecksum(heartbeatMessageBytes, timestamp), MachineLinkType.Server,
                                PackType.Heartbeat, timestamp);
                            ctx.WriteAndFlushAsync(new NetworkVerify(heartbeatMessageBytes, header));
                        }
                        break;
                    case IdleState.AllIdle:
                        if (networkService.AllChannels.TryGetValue(machineKey.MachineId, out var channel))
                        {
                            channelCloseSwitch.ModifyChannelCloseState(machineKey.MachineId);
                            channel.Dispose();
                        }
                        break;
                }
            }
        }
        else
        {
            ctx.Channel.DisconnectAsync();
        }

        base.UserEventTriggered(ctx, evt);
    }

    public override void ChannelActive(IChannelHandlerContext context)
    {
        // 链接成功向客户端发送携带自身设备Id的心跳包
        ServerMachineId = Guid.Parse("0a5bdde6-fb04-4313-83be-2c1146a5438b");
        var heartbeatMessageJson = NetworkHeartbeat.GenerateJson(ServerMachineId);
        var heartbeatMessageBytes = Encoding.UTF8.GetBytes(heartbeatMessageJson);
        var timestamp = PacketHeader.ConvertToUnixTimestamp();
        var header = PacketHeader.Create(PacketHeader.VersionConst, MessageType.Response, heartbeatMessageBytes.Length,
            PacketHeader.GenerateChecksum(heartbeatMessageBytes, timestamp), MachineLinkType.Server,
            PackType.Heartbeat, timestamp);
        context.WriteAndFlushAsync(new NetworkVerify(heartbeatMessageBytes, header));
        base.ChannelActive(context);
    }

    [Description("客户端断开连接时触发")]
    public override void ChannelInactive(IChannelHandlerContext ctx)
    {
        var clientLinkChannel = new ClientLinkChannel(MachineLinkType.Client, ctx.Channel);
        SendLogBusModel.Text = $"{ctx.Channel.RemoteAddress}：断开连接";
        MessageBusUtil.SendMessage(SendLogBusModel, MessageBusContract.MessageBusService);
        MessageBusUtil.SendMessage(new MachineExitBusModel
        {
            ClientLinkChannel = clientLinkChannel
        }, MessageBusContract.MessageBusService);
        contentPanelViewModel.RemoveHostInfo(clientLinkChannel);
        var machineKey = ctx.Channel.GetAttribute(ChannelAttributes.MachineKey).Get();
        if (machineKey != null)
        {
            if (networkService.AllChannels.TryGetValue(machineKey.MachineId, out var currentChannel))
            {
                channelCloseSwitch.ModifyChannelCloseState(machineKey.MachineId);
                currentChannel.Dispose();
                networkService.AllChannels.TryRemove(machineKey.MachineId, out _);
            }
        }

        base.ChannelInactive(ctx);
    }

    public override async Task CloseAsync(IChannelHandlerContext context)
    {
        var machineKey = context.Channel.GetAttribute(ChannelAttributes.MachineKey).Get();
        if (machineKey != null)
        {
            var channelCloseState = channelCloseSwitch.GetChannelCloseState(machineKey.MachineId);
            if (channelCloseState.IsClose)
            {
                channelCloseState.IsClose = false;
                await base.CloseAsync(context); 
            }
        }
    }
}