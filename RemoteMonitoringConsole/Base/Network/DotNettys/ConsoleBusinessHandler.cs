using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
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
using RemoteMonitoringConsole.Base.MessageBusModels;
using RemoteMonitoringConsole.Base.Network.Handlers;
using RemoteMonitoringConsole.Views;

namespace RemoteMonitoringConsole.Base.Network.DotNettys;

[AsType(LifetimeEnum.SingleInstance)]
public partial class ConsoleBusinessHandler(
    IMediator mediator,
    IConsoleNetworkService consoleNetworkService) : SimpleChannelInboundHandler<NetworkVerify>
{
    private int _reconnectAttempts = 0;
    private const int MaxRetries = 5;
    private const int BaseDelay = 2;
    private readonly Random _jitter = new();

    private WriteableBitmap? _bitmap;

    private readonly Stack<ImageDrawing> _drawingPool = new(3);

    public required MainWindow CurrentWindow { get; set; }
    
    private Guid CurrentMachineId => Guid.Parse("483808da-789f-4b9e-b76e-78f1e0a098cb");
    
    private Guid ServerMachineId { get; set; } = Guid.Parse("0a5bdde6-fb04-4313-83be-2c1146a5438b");

    protected override async void ChannelRead0(IChannelHandlerContext ctx, NetworkVerify msg)
    {
        try
        {
            if (msg.PacketHeader.DataLength == 0) return;
            var msgHeader = msg.PacketHeader;
            var payload = msg.ReceiveMessageBytes;
            var verifyNetworkMessageResponse =
                mediator.SendAsync<VerifyNetworkMessageCommand, VerifyNetworkMessageResponse>(
                    new VerifyNetworkMessageCommand
                    {
                        Msg = msg
                    }).Result;

            //收到空数据包直接return
            if (!verifyNetworkMessageResponse.IsVerify)
            {
                return;
            }

            if (HandleHeartbeatPackets(ctx, msg, msgHeader)) return;

            switch (msgHeader.MessageType)
            {
                case MessageType.Command:
                    break;
                case MessageType.Response:
                    var message = Encoding.UTF8.GetString(payload);
                    var networkResponse = JsonConvert.DeserializeObject<NetworkResponse>(message);
                    if (networkResponse is not null)
                    {
                        switch (networkResponse.CommandType)
                        {
                            case CommandType.ObtainScreen:
                                var primaryScreen = CurrentWindow.Screens.All.First(s => s.IsPrimary);
                                var rect = primaryScreen.Bounds;
                                _bitmap = await Task.Run(() =>
                                {
                                    using var stream = new MemoryStream(networkResponse.Data);
                                    return WriteableBitmap.Decode(stream);
                                });
                                // 更新属性
                                DispatcherUiThreadInvokeAsync(() =>
                                {
                                    if (!_drawingPool.TryPop(out var drawing))
                                    {
                                        drawing = new ImageDrawing();
                                    }

                                    drawing.Rect = new Rect(rect.X, rect.Y, rect.Width, rect.Height);
                                    drawing.ImageSource = _bitmap;
                                    CurrentWindow.ViewModel.MonitoringBoardPanelView.ViewModel.ControlImage ??= new DrawingImage();
                                    if (CurrentWindow.ViewModel.MonitoringBoardPanelView.ViewModel.ControlImage!.Drawing is ImageDrawing imageDrawing)
                                    {
                                        _drawingPool.Push(imageDrawing);
                                    }

                                    CurrentWindow.ViewModel.MonitoringBoardPanelView.ViewModel.ControlImage.Drawing = drawing;
                                });
                                break;
                            case CommandType.SendTerminalCommand:
                                var str = Encoding.UTF8.GetString(networkResponse.Data);
                                MessageBusUtil.SendMessage(new TerminalCommandOutputBusModel
                                {
                                    Output = str
                                }, MessageBusContract.MessageBusConsole);
                                break;
                        }
                    }

                    break;
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }

    [Description("通道激活后即可发送一条信息")]
    public override void ChannelActive(IChannelHandlerContext context)
    {
        var heartbeatMessageJson = NetworkHeartbeat.GenerateJson(CurrentMachineId);
        var heartbeatMessageBytes = Encoding.UTF8.GetBytes(heartbeatMessageJson);
        var timestamp = PacketHeader.ConvertToUnixTimestamp();
        var header = PacketHeader.Create(PacketHeader.VersionConst, MessageType.Response, heartbeatMessageBytes.Length,
            PacketHeader.GenerateChecksum(heartbeatMessageBytes, timestamp), MachineLinkType.Console,
            PackType.Heartbeat, timestamp);
        context.WriteAndFlushAsync(new NetworkVerify(heartbeatMessageBytes, header));
        base.ChannelActive(context);
    }

    [Description("心跳处理")]
    public override void UserEventTriggered(IChannelHandlerContext ctx, object evt)
    {
        if (evt is IdleStateEvent idleEvent)
        {
            switch (idleEvent.State)
            {
                case IdleState.ReaderIdle:
                    break;
                case IdleState.WriterIdle:
                    var heartbeatMessageJson = NetworkHeartbeat.GenerateJson(CurrentMachineId);
                    var heartbeatMessageBytes = Encoding.UTF8.GetBytes(heartbeatMessageJson);
                    var timestamp = PacketHeader.ConvertToUnixTimestamp();
                    var header = PacketHeader.Create(PacketHeader.VersionConst, MessageType.Response,
                        heartbeatMessageBytes.Length,
                        PacketHeader.GenerateChecksum(heartbeatMessageBytes, timestamp), MachineLinkType.Console,
                        PackType.Heartbeat, timestamp);
                    ctx.WriteAndFlushAsync(new NetworkVerify(heartbeatMessageBytes, header));
                    break;
                case IdleState.AllIdle:
                    break;
            }
        }
        else
        {
            base.UserEventTriggered(ctx, evt);
        }
    }

    public override Task CloseAsync(IChannelHandlerContext context)
    {
        if (context.Channel.Active)
        {
            return Task.CompletedTask; 
        }
        return base.CloseAsync(context);
    }

    [Description("异常捕获")]
    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
    }
}