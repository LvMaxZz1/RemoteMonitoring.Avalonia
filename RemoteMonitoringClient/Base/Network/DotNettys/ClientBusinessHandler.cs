using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
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
using RemoteMonitoringClient.Base.Network.Handlers;
using RemoteMonitoringClient.Views;

namespace RemoteMonitoringClient.Base.Network.DotNettys;

[AsType(LifetimeEnum.SingleInstance)]
public partial class ClientBusinessHandler(IMediator mediator) : SimpleChannelInboundHandler<NetworkVerify>
{
    private Power? _power = Power.Off;

    private WriteableBitmap? _bitmap;

    private PixelRect? _pixelRect;

    const string startMarker = "echo [CURRENT_PATH_START]";

    public MainWindow Main { get; set; }

    private CancellationTokenSource _cts;

    private Guid ServerMachineId { get; set; } = Guid.Parse("0a5bdde6-fb04-4313-83be-2c1146a5438b");

    protected override async void ChannelRead0(IChannelHandlerContext ctx, NetworkVerify msg)
    {
        var msgHeader = msg.PacketHeader;
        var payload = msg.ReceiveMessageBytes;
        var verifyNetworkMessageResponse =
            mediator.SendAsync<VerifyNetworkMessageCommand, VerifyNetworkMessageResponse>(
                new VerifyNetworkMessageCommand
                {
                    Msg = msg
                }).Result;
        if (!verifyNetworkMessageResponse.IsVerify) return;

        if (HandleHeartbeatPackets(ctx, msg, msgHeader)) return;

        var message = Encoding.UTF8.GetString(payload);
        var networkCommand = JsonConvert.DeserializeObject<NetworkCommand>(message);
        // 验证客户端和数据操作
        if (networkCommand is not null)
        {
            if (networkCommand.CommandType <= CommandType.Logout)
            {
                WindowsApiHelper.ExecuteNetworkCommand(networkCommand.CommandType);
            }
            else
            {
                switch (networkCommand.CommandType)
                {
                    case CommandType.ObtainScreen:
                        var primaryScreen = Main.Screens.All.First(s => s.IsPrimary);
                        _pixelRect ??= primaryScreen.Bounds;
                        _bitmap ??= new WriteableBitmap(
                            new PixelSize(_pixelRect.Value.Width, _pixelRect.Value.Height),
                            new Vector(_pixelRect.Value.X, _pixelRect.Value.Y),
                            PixelFormat.Rgba8888,
                            AlphaFormat.Premul);
                        if (networkCommand.ScreenInfo is not null && networkCommand.ScreenInfo.Power == Power.On)
                        {
                            var oldPower = _power;
                            _power = networkCommand.ScreenInfo.Power;
                            if (oldPower == Power.Off)
                            {
                                _cts = new CancellationTokenSource();
                                _ = StartCaptureLoop(ctx.Channel, _cts.Token);
                            }

                            var screenInfo = networkCommand.ScreenInfo;
                            if (screenInfo.Mouse.DwFlagsOne != 0 && screenInfo.Mouse.Dx != 0 &&
                                screenInfo.Mouse.Dy != 0 && !screenInfo.Mouse.IsDouble)
                                Win32Api.mouse_event(screenInfo.Mouse.DwFlagsOne, screenInfo.Mouse.Dx,
                                    screenInfo.Mouse.Dy, 0, 0);

                            if (screenInfo.KeyBd != null)
                            {
                                Win32Api.keybd_event(screenInfo.KeyBd.BVk, screenInfo.KeyBd.BScan,
                                    screenInfo.KeyBd.DwFlags, 0);
                                Win32Api.keybd_event(screenInfo.KeyBd.BVk, 0, (int)KEYEVENTF.KEYUP, 0);
                            }
                        }
                        else if (networkCommand.ScreenInfo is not null && networkCommand.ScreenInfo.Power == Power.Off)
                        {
                            _power = Power.Off;
                            await _cts.CancelAsync();
                        }

                        break;
                    case CommandType.ObtainFiles:
                        break;
                    case CommandType.TransferFiles:
                        break;
                    case CommandType.SendTerminalCommand:
                        var tempFile = Path.GetTempFileName();
                        var command = $"{networkCommand.TerminalCommand} & echo [CURRENT_PATH_START] & cd & echo [CURRENT_PATH_END]";
                        var psi = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = $"/c ({command}) > \"{tempFile}\" 2>&1",
                            Verb = "runas",
                            UseShellExecute = true,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden
                        };
                        using (var process = Process.Start(psi))
                        {
                            if (process != null) await process.WaitForExitAsync();
                        }
                        var fullOutput = await File.ReadAllTextAsync(tempFile, Encoding.GetEncoding("GBK"));
                        File.Delete(tempFile);
                        
                        var lines = fullOutput.Split(["\r\n", "\n"], StringSplitOptions.None);
                        var filteredLines = lines
                            .Where(line => !line.Trim().Equals("[CURRENT_PATH_START]", StringComparison.OrdinalIgnoreCase)
                                           && !line.Trim().Equals("[CURRENT_PATH_END]", StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        var result = string.Join(Environment.NewLine, filteredLines).Trim();

                        var bytes = Encoding.UTF8.GetBytes(result);
                        await SendNetworkResponse(bytes, CommandType.SendTerminalCommand, ctx.Channel);
                        // var process = new Process
                        // {
                        //     StartInfo = new ProcessStartInfo
                        //     {
                        //         FileName = "cmd.exe", // 或 powershell.exe
                        //         Arguments = "",
                        //         RedirectStandardInput = true,
                        //         RedirectStandardOutput = true,
                        //         RedirectStandardError = true,
                        //         UseShellExecute = false,
                        //         CreateNoWindow = true,
                        //         Verb = "runas",
                        //         StandardOutputEncoding = Encoding.GetEncoding("GBK"), // 确保中文不乱码
                        //         StandardErrorEncoding = Encoding.GetEncoding("GBK")
                        //     },
                        //     EnableRaisingEvents = true
                        // };
                        // var outputBuilder = new StringBuilder();
                        // var errorBuilder = new StringBuilder();
                        // process.OutputDataReceived += (sender, args) =>
                        // {
                        //     if (args.Data != null)
                        //     {
                        //         outputBuilder.AppendLine(args.Data);
                        //     }
                        // };
                        //
                        // process.ErrorDataReceived += (sender, args) =>
                        // {
                        //     if (args.Data != null)
                        //     {
                        //         errorBuilder.AppendLine(args.Data);
                        //     }
                        // };
                        // process.Start();
                        // process.BeginOutputReadLine();
                        // process.BeginErrorReadLine();
                        //
                        // await process.StandardInput.WriteLineAsync(networkCommand.TerminalCommand);
                        //
                        // await process.StandardInput.WriteLineAsync("echo [CURRENT_PATH_START]");
                        // await process.StandardInput.WriteLineAsync("cd");
                        // await process.StandardInput.WriteLineAsync("echo [CURRENT_PATH_END]");
                        //
                        // await process.StandardInput.WriteLineAsync("exit");
                        // await process.WaitForExitAsync();
                        //
                        // var fullOutput = outputBuilder + errorBuilder.ToString();
                        // int markerIndex = fullOutput.IndexOf(startMarker);
                        //
                        // if (markerIndex != -1)
                        // {
                        //     fullOutput = fullOutput.Substring(0, markerIndex);
                        // }
                        //
                        // var lines = fullOutput.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                        //
                        // if (lines.Length > 0 && lines[^1].TrimEnd().EndsWith(">"))
                        // {
                        //     fullOutput = string.Join(Environment.NewLine, lines, 0, lines.Length - 1).TrimEnd();
                        // }
                        // else
                        // {
                        //     fullOutput = fullOutput.TrimEnd();
                        // }
                        // var bytes = Encoding.UTF8.GetBytes(fullOutput);
                        // await SendNetworkResponse(bytes, CommandType.SendTerminalCommand, ctx.Channel);
                        break;
                    case CommandType.AvaloniaShutdown:
                        if (Application.Current != null)
                        {
                            var lifetime =
                                Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
                            lifetime?.Shutdown();
                        }

                        break;
                }
            }
        }
    }

    private static void CaptureWindows(IntPtr destPtr, PixelRect rect)
    {
        var hdcSrc = Win32Api.GetDC(IntPtr.Zero);
        var hdcDest = Win32Api.CreateCompatibleDC(hdcSrc);
        var hBitmap = Win32Api.CreateCompatibleBitmap(hdcSrc, rect.Width, rect.Height);

        Win32Api.SelectObject(hdcDest, hBitmap);
        Win32Api.BitBlt(hdcDest, 0, 0, rect.Width, rect.Height,
            hdcSrc, rect.X, rect.Y, 0x00CC0020);

        var bmpInfo = new Win32Api.BITMAPINFO
        {
            bmiHeader = new Win32Api.BITMAPINFOHEADER
            {
                biSize = (uint)Marshal.SizeOf<Win32Api.BITMAPINFOHEADER>(),
                biWidth = rect.Width,
                biHeight = -rect.Height, // 避免图像倒置
                biPlanes = 1,
                biBitCount = 32,
                biCompression = 0 // BI_RGB
            }
        };
        Win32Api.GetDIBits(hdcDest, hBitmap, 0, (uint)rect.Height,
            destPtr, ref bmpInfo, 0);

        Win32Api.DeleteObject(hBitmap);
        Win32Api.DeleteDC(hdcDest);
        Win32Api.ReleaseDC(IntPtr.Zero, hdcSrc);
    }

    public override void ChannelInactive(IChannelHandlerContext ctx)
    {
        for (byte vk = 0x01; vk <= 0xFE; vk++)
        {
            Win32Api.keybd_event(vk, 0, (int)KEYEVENTF.KEYUP, 0);
        }

        base.ChannelInactive(ctx);
    }

    public override void ChannelActive(IChannelHandlerContext context)
    {
        var heartbeatMessageJson = NetworkHeartbeat.GenerateJson(Guid.Parse("6ad94c0e-9031-4423-9ee8-1cc2c3ff10c9"));
        var heartbeatMessageBytes = Encoding.UTF8.GetBytes(heartbeatMessageJson);
        var timestamp = PacketHeader.ConvertToUnixTimestamp();
        var header = PacketHeader.Create(PacketHeader.VersionConst, MessageType.Response, heartbeatMessageBytes.Length,
            PacketHeader.GenerateChecksum(heartbeatMessageBytes, timestamp), MachineLinkType.Client,
            PackType.Heartbeat, timestamp);
        context.WriteAndFlushAsync(new NetworkVerify(heartbeatMessageBytes, header));
        base.ChannelActive(context);
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
    }


    public override void UserEventTriggered(IChannelHandlerContext ctx, object evt)
    {
        if (evt is IdleStateEvent idleEvent)
        {
            switch (idleEvent.State)
            {
                case IdleState.ReaderIdle:
                    ctx.CloseAsync();
                    break;
                case IdleState.WriterIdle:
                    var heartbeatMessageJson =
                        NetworkHeartbeat.GenerateJson(Guid.Parse("6ad94c0e-9031-4423-9ee8-1cc2c3ff10c9"));
                    var heartbeatMessageBytes = Encoding.UTF8.GetBytes(heartbeatMessageJson);
                    var timestamp = PacketHeader.ConvertToUnixTimestamp();
                    var header = PacketHeader.Create(PacketHeader.VersionConst, MessageType.Response,
                        heartbeatMessageBytes.Length,
                        PacketHeader.GenerateChecksum(heartbeatMessageBytes, timestamp), MachineLinkType.Client,
                        PackType.Heartbeat, timestamp);
                    ctx.WriteAndFlushAsync(new NetworkVerify(heartbeatMessageBytes, header));
                    break;
                case IdleState.AllIdle:
                    // 服务端秒未响应，判定为下线
                    ctx.CloseAsync();
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
}