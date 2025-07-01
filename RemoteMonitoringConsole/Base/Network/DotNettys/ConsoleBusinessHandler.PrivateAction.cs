using System;
using System.ComponentModel;
using System.Text;
using Avalonia.Threading;
using DotNetty.Transport.Channels;
using Newtonsoft.Json;
using RemoteMonitoring.Core.Services.Networks.Base;
using RemoteMonitoring.Core.Services.Networks.Base.Messages;
using RemoteMonitoring.Core.Services.Networks.Base.SocketPackets;

namespace RemoteMonitoringConsole.Base.Network.DotNettys;

public partial class ConsoleBusinessHandler
{
    private void DispatcherUiThreadInvokeAsync(Action action)
    {
        Dispatcher.UIThread.InvokeAsync(action);
    }

    [Description("处理心跳包")]
    private bool HandleHeartbeatPackets(IChannelHandlerContext ctx, NetworkVerify msg, PacketHeader msgHeader)
    {
        if (msgHeader.PackType == PackType.Heartbeat)
        {
            var heartbeatMessageJson = Encoding.UTF8.GetString(msg.ReceiveMessageBytes);
            var heartbeatMessage = JsonConvert.DeserializeObject<NetworkHeartbeat>(heartbeatMessageJson);
            if (heartbeatMessage != null &&  heartbeatMessage.HeartbeatMachineId == ServerMachineId)
            {
                var machineKey = ctx.Channel.GetAttribute(ChannelAttributes.MachineKey).Get();
                if (machineKey == null || machineKey.MachineId != ServerMachineId)
                {
                    ctx.Channel.GetAttribute(ChannelAttributes.MachineKey).Set(MachineKey.Create(heartbeatMessage.HeartbeatMachineId));
                }
            }
            return true;
        }

        return false;
    }
    
    [Description("计算重连回退时间")]
    private TimeSpan CalculateBackoffDelay()
    {
        // 基础指数退避 + 随机抖动（30%范围）
        var baseDelay = BaseDelay * Math.Pow(2, _reconnectAttempts);
        var jitter = _jitter.NextDouble() * 0.3 * baseDelay; 
        return TimeSpan.FromSeconds(baseDelay + jitter);
    }
    
    private async void ConnectAsync()
    {
        await consoleNetworkService.AgainConnectAsync();
    }
}