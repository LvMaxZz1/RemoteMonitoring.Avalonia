using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Newtonsoft.Json;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Services.Networks.Base;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;
using RemoteMonitoring.Core.Services.Networks.Base.Messages;
using RemoteMonitoring.Core.Services.Networks.Base.SocketPackets;
using SkiaSharp;

namespace RemoteMonitoringClient.Base.Network.DotNettys;

public partial class ClientBusinessHandler
{
    private async Task StartCaptureLoop(IChannel channel, CancellationToken cancellationToken)
    {
        while (_power != Power.Off)
        {
            // 异步获取截图（避免阻塞事件循环）
            var screenshot = await Task.Run(CaptureDesktop, cancellationToken);
            await SendNetworkResponse(screenshot, CommandType.ObtainScreen, channel, cancellationToken);
            await Task.Delay(40, cancellationToken);
            ByteArrayPool.Return(screenshot);
        }
    }

    [Description("发送响应包")]
    private async Task SendNetworkResponse(byte[] data, CommandType commandType, IChannel channel,
        CancellationToken cancellationToken = default)
    {
        var networkResponseJson =
            NetworkResponse.GenerateJson(commandType, Guid.Parse("483808da-789f-4b9e-b76e-78f1e0a098cb"), data);
        var networkResponseBytes = Encoding.UTF8.GetBytes(networkResponseJson);
        var timestamp = PacketHeader.ConvertToUnixTimestamp();
        var header = PacketHeader.Create(PacketHeader.VersionConst, MessageType.Response, networkResponseBytes.Length,
            PacketHeader.GenerateChecksum(networkResponseBytes, timestamp), MachineLinkType.Client,
            PackType.Default, timestamp);
        await channel.WriteAndFlushAsync(new NetworkVerify(networkResponseBytes, header));
    }

    private async Task<byte[]> CaptureDesktop()
    {
        if (_bitmap == null || _pixelRect == null) return [];
        using var buffer = _bitmap.Lock();
        if (OperatingSystem.IsWindows())
        {
            CaptureWindows(buffer.Address, _pixelRect.Value);
        }

        var pixelData = new byte[_bitmap.PixelSize.Width * _bitmap.PixelSize.Height * 4];
        Marshal.Copy(buffer.Address, pixelData, 0, pixelData.Length);
        var info = new SKImageInfo(
            _bitmap.PixelSize.Width,
            _bitmap.PixelSize.Height,
            SKColorType.Bgra8888,
            SKAlphaType.Premul
        );
        using var skImage = SKImage.FromPixelCopy(info, pixelData);
        // 压缩操作
        using var ms = new MemoryStream();
        using (var skData = skImage.Encode(SKEncodedImageFormat.Jpeg, 50))
        {
            skData.SaveTo(ms);
        }

        var dataMemory = await ByteArrayPool.RentAsync((int)ms.Length);
        dataMemory = ms.ToArray();
        return dataMemory;
    }

    [Description("处理心跳包")]
    private bool HandleHeartbeatPackets(IChannelHandlerContext ctx, NetworkVerify msg, PacketHeader msgHeader)
    {
        if (msgHeader.PackType == PackType.Heartbeat)
        {
            var heartbeatMessageJson = Encoding.UTF8.GetString(msg.ReceiveMessageBytes);
            var heartbeatMessage = JsonConvert.DeserializeObject<NetworkHeartbeat>(heartbeatMessageJson);
            if (heartbeatMessage != null && heartbeatMessage.HeartbeatMachineId == ServerMachineId)
            {
                var machineKey = ctx.Channel.GetAttribute(ChannelAttributes.MachineKey).Get();
                if (machineKey == null || machineKey.MachineId != ServerMachineId)
                {
                    ctx.Channel.GetAttribute(ChannelAttributes.MachineKey)
                        .Set(MachineKey.Create(heartbeatMessage.HeartbeatMachineId));
                }
            }

            return true;
        }

        return false;
    }
}