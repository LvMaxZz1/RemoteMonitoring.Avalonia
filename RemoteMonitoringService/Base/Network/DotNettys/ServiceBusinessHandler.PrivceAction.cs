using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Services.Networks.Base;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;
using RemoteMonitoring.Core.Services.Networks.Base.Messages;
using RemoteMonitoring.Core.Services.Networks.Base.SocketPackets;
using RemoteMonitoring.Core.Utils;
using RemoteMonitoringService.Base.MessageBusModels;

namespace RemoteMonitoringService.Base.Network.DotNettys;

public partial class ServiceBusinessHandler
{
    [Description("转发响应")]
    private async Task ForwardNetworkResponse(NetworkResponse networkResponse, byte[] payload,
        ClientLinkChannel clientChannel)
    {
        if (networkService.AllChannels.TryGetValue(networkResponse.ConsoleMachineId, out var consoleChannel) &&
            consoleChannel is { Channel.Active: true, LinkType: MachineLinkType.Console })
        {
            var timestamp = PacketHeader.ConvertToUnixTimestamp();
            var header = PacketHeader.Create(
                PacketHeader.VersionConst, MessageType.Response, payload.Length,
                PacketHeader.GenerateChecksum(payload, timestamp), clientChannel.LinkType, PackType.Default,
                timestamp);
            await consoleChannel.Channel.WriteAndFlushAsync(new NetworkVerify(payload, header));
        }
    }

    [Description("处理心跳包")]
    private bool HandleHeartbeatPackets(NetworkVerify msg, PacketHeader msgHeader, ClientLinkChannel linkChannel)
    {
        if (msgHeader.PackType == PackType.Heartbeat)
        {
            // 验证心跳包携带Id是否符合身份
            var heartbeatMessageJson = Encoding.UTF8.GetString(msg.ReceiveMessageBytes);
            var heartbeatMessage = JsonConvert.DeserializeObject<NetworkHeartbeat>(heartbeatMessageJson);
            if (heartbeatMessage != null && networkService.AllChannels.TryAdd(heartbeatMessage.HeartbeatMachineId, linkChannel))
            {
                linkChannel.MachineId = heartbeatMessage.HeartbeatMachineId;
                LinkMachineId = heartbeatMessage.HeartbeatMachineId;
                linkChannel.Channel?.GetAttribute(ChannelAttributes.MachineKey)
                    .Set(MachineKey.Create(heartbeatMessage.HeartbeatMachineId));
                var clientLinkChannel = new ClientLinkChannel(msgHeader.LinkType, linkChannel.Channel);
                if (channelCloseSwitch.ChannelCloseStates.FirstOrDefault(x =>
                        x.MachineId == heartbeatMessage.HeartbeatMachineId) == null)
                {
                    channelCloseSwitch.ChannelCloseStates.Add(new ChannelCloseState(false, heartbeatMessage.HeartbeatMachineId));
                }
                contentPanelViewModel.AddHostInfo(clientLinkChannel);
                SendLogBusModel.Text = $"{clientLinkChannel.Channel?.RemoteAddress}： 连接服务器成功";
                MessageBusUtil.SendMessage(SendLogBusModel, MessageBusContract.MessageBusService);
                MessageBusUtil.SendMessage(new MachineOnlineBusModel
                {
                    ClientLinkChannel = clientLinkChannel
                }, MessageBusContract.MessageBusService);
            }

            return true;
        }

        return false;
    }

    [Description("转发命令")]
    private async void ExecuteNetworkCommand(ClientLinkChannel consoleChannel, NetworkCommand networkCommand,
        byte[] payload)
    {
        if (networkService.AllChannels.TryGetValue(networkCommand.ClientMachineId, out var clientChannel) &&
            clientChannel is { LinkType: MachineLinkType.Client, Channel.Active: true })
        {
            var timestamp = PacketHeader.ConvertToUnixTimestamp();
            var header = PacketHeader.Create(
                PacketHeader.VersionConst, MessageType.Command, payload.Length,
                PacketHeader.GenerateChecksum(payload, timestamp), consoleChannel.LinkType,
                PackType.Default,
                timestamp);
            await clientChannel.Channel.WriteAndFlushAsync(new NetworkVerify(payload, header));
        }
    }
}