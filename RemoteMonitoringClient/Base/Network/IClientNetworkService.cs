using System;
using System.Net;
using System.Net.Sockets;
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
using RemoteMonitoringClient.Base.Network.DotNettys;
using RemoteMonitoringClient.Views;

namespace RemoteMonitoringClient.Base.Network;

public interface IClientNetworkService : INetworkService
{
    public ClientLinkChannel? ClientLinkChannel { get; }
}

[AsType(LifetimeEnum.SingleInstance)]
public class ClientNetworkService(IServiceProvider serviceProvider, ClientNetworkSetting clientNetworkSetting) : IClientNetworkService
{
    public ClientLinkChannel? ClientLinkChannel { get; set; }

    public async Task InitLinkAsync()
    {
        try
        {
            var allocator = new PooledByteBufferAllocator(
                preferDirect: false, // 优先使用直接内存（堆外内存），减少GC压力[2,8](@ref)
                nHeapArena: 5, // 禁用堆内存（完全依赖直接内存）
                nDirectArena: 5, // Arena数量=CPU核心数*2（上限32）[5,7](@ref)
                pageSize: 8192, // 页大小8KB，与操作系统内存页对齐[4,5](@ref)
                maxOrder: 11, // 每个Chunk包含2^11=2048页，总大小=8KB*2048=16MB[5](@ref)
                tinyCacheSize: 0, // 禁用Tiny缓存（jemalloc4已弃用Tiny类型）[5](@ref)
                smallCacheSize: 256, // 每个线程的Small缓存条目数（高频小对象）[8](@ref)
                normalCacheSize: 64 // 每个线程的Normal缓存条目数（中等大小对象）[8](@ref)
            );
            var group = new MultithreadEventLoopGroup();
            var bootstrap = new Bootstrap();
            bootstrap.Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.ConnectTimeout, TimeSpan.FromSeconds(10))
                .Option(ChannelOption.SoReuseaddr, true)
                .Option(ChannelOption.Allocator, allocator)
                .Option(ChannelOption.SoSndbuf, 150000 * 2)
                .Option(ChannelOption.SoRcvbuf, 150000 * 2)
                .Option(
                    ChannelOption.RcvbufAllocator,
                    new AdaptiveRecvByteBufAllocator(4096, 200000, 300000) // 最小、初始、最大容量
                )
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    var scope = serviceProvider.CreateScope();
                    var packetHeaderDecoder = scope.ServiceProvider.GetRequiredService<PacketHeaderDecoder>();
                    var packetHeaderEncoder = scope.ServiceProvider.GetRequiredService<PacketHeaderEncoder>();
                    var clientBusinessHandler = scope.ServiceProvider.GetRequiredService<ClientBusinessHandler>();
                    var window = scope.ServiceProvider.GetRequiredService<MainWindow>();
                    clientBusinessHandler.Main = window;
                    var pipeline = channel.Pipeline;
                    pipeline
                        //.AddLast(new IdleStateHandler(40, 20, 60))
                        .AddLast("zlibDecoder", ZlibCodecFactory.NewZlibDecoder(ZlibWrapper.Gzip))
                        .AddLast("decoder", packetHeaderDecoder)
                        .AddLast("zlibEncoder", ZlibCodecFactory.NewZlibEncoder(ZlibWrapper.Gzip))
                        .AddLast("encoder", packetHeaderEncoder)
                        .AddLast("clientBusinessHandler", clientBusinessHandler);
                }));
            var channel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(clientNetworkSetting.IpAddress), clientNetworkSetting.Port));
            ClientLinkChannel = new ClientLinkChannel(MachineLinkType.Client, channel);
        }
        catch (Exception e)
        {
            if (e is SocketException or OperationCanceledException)
            {
                return;
            }

            if (ClientLinkChannel?.Channel != null)
            {
                await ClientLinkChannel.Channel.DisconnectAsync();
                await ClientLinkChannel.Channel.CloseAsync();
            }

            throw;
        }
    }

    public async Task ReleaseLinkAsync()
    {
        try
        {
            if (ClientLinkChannel is { Channel.Active: true })
            {
                await ClientLinkChannel.Channel.CloseAsync();
            }
        }
        catch
        {
            //
        }
    }
}