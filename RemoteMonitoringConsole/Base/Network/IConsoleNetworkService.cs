using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs.Compression;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.DependencyInjection;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoring.Core.Services.Networks;
using RemoteMonitoring.Core.Services.Networks.Base;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;
using RemoteMonitoring.Core.Services.Networks.Base.Messages;
using RemoteMonitoring.Core.Services.Networks.Base.SocketPackets;
using RemoteMonitoringConsole.Base.Network.DotNettys;
using RemoteMonitoringConsole.Views;

namespace RemoteMonitoringConsole.Base.Network;

public interface IConsoleNetworkService : INetworkService
{
    public ClientLinkChannel? ConsoleLinkChannel { get; }

    [Description("重新链接")]
    Task AgainConnectAsync();

    /// <summary>
    /// 发送命令到客户端
    /// </summary>
    /// <param name="commandType"> 命令类型 </param>
    /// <param name="screenInfo"> 屏幕信息 如果命令类型为 ObtainScreen 则需要 </param>
    /// <param name="terminalCommand"> 终端命令 如果命令类型为 SendTerminalCommand 则需要 </param>
    /// <returns></returns>
    Task SendCommandToClient(CommandType commandType, ScreenInfo? screenInfo, string? terminalCommand = null);
}

[AsType(LifetimeEnum.SingleInstance, typeof(IConsoleNetworkService))]
public class ConsoleNetworkService(IServiceProvider serviceProvider, ConsoleNetworkSetting consoleNetworkSetting) : IConsoleNetworkService
{
    public ClientLinkChannel? ConsoleLinkChannel { get; set; }
    
    public MultithreadEventLoopGroup? LoopGroup { get; set; }

    public Bootstrap? Bootstrap { get; set; }

    public PooledByteBufferAllocator? ByteBufferAllocator { get; set; } = new(
        preferDirect: false, // 优先使用直接内存（堆外内存），减少GC压力[2,8](@ref)
        nHeapArena: 0, // 禁用堆内存（完全依赖直接内存）
        nDirectArena: 0, // Arena数量=CPU核心数*2（上限32）[5,7](@ref)
        pageSize: 8192, // 页大小8KB，与操作系统内存页对齐[4,5](@ref)
        maxOrder: 11, // 每个Chunk包含2^11=2048页，总大小=8KB*2048=16MB[5](@ref)
        tinyCacheSize: 0, // 禁用Tiny缓存（jemalloc4已弃用Tiny类型）[5](@ref)
        smallCacheSize: 256, // 每个线程的Small缓存条目数（高频小对象）[8](@ref)
        normalCacheSize: 64 // 每个线程的Normal缓存条目数（中等大小对象）[8](@ref)
    );

    public async Task InitLinkAsync()
    {
        try
        {
            Bootstrap = new Bootstrap();

            LoopGroup ??= new MultithreadEventLoopGroup();
            Bootstrap
                .Group(LoopGroup)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.ConnectTimeout, TimeSpan.FromSeconds(60))
                .Option(ChannelOption.Allocator, ByteBufferAllocator)
                .Option(ChannelOption.SoKeepalive, true)
                .Option(ChannelOption.SoSndbuf, 32768)
                .Option(ChannelOption.SoRcvbuf, 32768)
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    var scope = serviceProvider.CreateScope();
                    var packetHeaderDecoder = scope.ServiceProvider.GetRequiredService<PacketHeaderDecoder>();
                    var packetHeaderEncoder = scope.ServiceProvider.GetRequiredService<PacketHeaderEncoder>();
                    var consoleBusinessHandler = scope.ServiceProvider.GetRequiredService<ConsoleBusinessHandler>();
                    var mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
                    consoleBusinessHandler.CurrentWindow = mainWindow;
                    var pipeline = channel.Pipeline;
                    pipeline
                        //.AddLast(new IdleStateHandler(40, 20, 60))
                        .AddLast("zlibDecoder", ZlibCodecFactory.NewZlibDecoder(ZlibWrapper.Gzip))
                        .AddLast("decoder", packetHeaderDecoder)
                        .AddLast("zlibEncoder", ZlibCodecFactory.NewZlibEncoder(ZlibWrapper.Gzip))
                        .AddLast("encoder", packetHeaderEncoder)
                        .AddLast("consoleBusiness", consoleBusinessHandler);
                }));
            var addresses = await Dns.GetHostAddressesAsync(consoleNetworkSetting.HostAddress);
            var ipv4 = addresses.First(a => a.AddressFamily == AddressFamily.InterNetwork);
            var channel = await Bootstrap.ConnectAsync(new IPEndPoint(ipv4, consoleNetworkSetting.Port));
            ConsoleLinkChannel = new ClientLinkChannel(MachineLinkType.Console, channel);
        }
        catch
        {
            await ReleaseLinkAsync();
        }
    }

    public async Task ReleaseLinkAsync()
    {
        LoopGroup = null;
        Bootstrap = null;
        if (ConsoleLinkChannel is { Channel.Active: true })
        {
            ConsoleLinkChannel.Dispose();
        }

        await Task.CompletedTask;
    }

    public async Task AgainConnectAsync()
    {
        try
        {
            if (Bootstrap != null)
            {
                var newChannel = await Bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(consoleNetworkSetting.HostAddress),
                    consoleNetworkSetting.Port));
                if (ConsoleLinkChannel?.Channel != null)
                {
                    await ConsoleLinkChannel.Channel.CloseAsync();
                    ConsoleLinkChannel.Channel = newChannel;
                }
            }
        }
        catch
        {
            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Close();
        }
        
    }
    
    [Description("发送命令给客户端")]
    public async Task SendCommandToClient(CommandType commandType, ScreenInfo? screenInfo, string? terminalCommand = null)
    {
        try
        {
            if (ConsoleLinkChannel is { Channel: not null })
            {
                var networkCommandJson = NetworkCommand.GenerateJson(commandType,
                    Guid.Parse("6ad94c0e-9031-4423-9ee8-1cc2c3ff10c9"),
                    Guid.Parse("483808da-789f-4b9e-b76e-78f1e0a098cb"), screenInfo, terminalCommand);
                var networkCommandBytes = Encoding.UTF8.GetBytes(networkCommandJson);
                var timestamp = PacketHeader.ConvertToUnixTimestamp();
                var header = PacketHeader.Create(PacketHeader.VersionConst, MessageType.Command,
                    networkCommandBytes.Length,
                    PacketHeader.GenerateChecksum(networkCommandBytes, timestamp), MachineLinkType.Console,
                    PackType.Default, timestamp);
                await ConsoleLinkChannel.Channel.WriteAndFlushAsync(new NetworkVerify(networkCommandBytes, header));
            }
        }
        catch
        {
            //
        }
    }
}