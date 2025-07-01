using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Codecs.Compression;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.DependencyInjection;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoring.Core.Services.Networks;
using RemoteMonitoring.Core.Services.Networks.Base;
using RemoteMonitoringService.Base.Network.DotNettys;

namespace RemoteMonitoringService.Base.Network;

public interface IServiceNetworkService : INetworkService
{
    ConcurrentDictionary<Guid, ClientLinkChannel> AllChannels { get; set; }
}

[AsType(LifetimeEnum.SingleInstance, typeof(IServiceNetworkService))]
public class ServiceNetworkService(IServiceProvider serviceProvider) : IServiceNetworkService
{
    private IChannel? _serverChannel { get; set; }

    public ConcurrentDictionary<Guid, ClientLinkChannel> AllChannels { get; set; } = [];

    private MultithreadEventLoopGroup? _bossGroup;

    private MultithreadEventLoopGroup? _workerGroup;

    public async Task InitLinkAsync()
    {
        try
        {
            var networkSetting = serviceProvider.GetRequiredService<ServiceNetworkSetting>();
            var pooledByteBufferAllocator = new PooledByteBufferAllocator(
                preferDirect: false,                // 优先使用直接内存（堆外内存），减少GC压力
                nHeapArena: 0,                     // 禁用堆内存（完全依赖直接内存）
                nDirectArena: 0, // Arena数量=CPU核心数*2（上限32）
                pageSize: 8192,                    // 页大小8KB，与操作系统内存页对齐[4,5]
                maxOrder: 11,                      // 每个Chunk包含2^11=2048页，总大小=8KB*2048=16MB
                tinyCacheSize: 0,                  // 禁用Tiny缓存（jemalloc4已弃用Tiny类型）
                smallCacheSize: 256,               // 每个线程的Small缓存条目数（高频小对象）
                normalCacheSize: 64                // 每个线程的Normal缓存条目数（中等大小对象）
            );
            _bossGroup ??= new MultithreadEventLoopGroup(1);
            _workerGroup ??= new MultithreadEventLoopGroup();
            var serverBootstrap = new ServerBootstrap();
            serverBootstrap.Group(_bossGroup, _workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 1024)
                .Option(ChannelOption.SoReuseaddr, true)
                .Option(ChannelOption.SoLinger, 5)
                .ChildOption(
                    ChannelOption.RcvbufAllocator, 
                    new AdaptiveRecvByteBufAllocator(4096, 65536, 400000) // 最小、初始、最大容量
                )
                .ChildOption(ChannelOption.SoKeepalive, true)
                .ChildOption(ChannelOption.Allocator, pooledByteBufferAllocator)
                .ChildOption(ChannelOption.TcpNodelay, true)
                .ChildOption(ChannelOption.SoSndbuf, 200000*2)
                .ChildOption(ChannelOption.SoRcvbuf, 200000*2)
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var scope = serviceProvider.CreateScope();
                    var packetHeaderDecoder = scope.ServiceProvider.GetRequiredService<PacketHeaderDecoder>();
                    var packetHeaderEncoder = scope.ServiceProvider.GetRequiredService<PacketHeaderEncoder>();
                    var serviceBusinessHandler = scope.ServiceProvider.GetRequiredService<ServiceBusinessHandler>();
                    var pipeline = channel.Pipeline;
                    pipeline
                        //.AddLast(new IdleStateHandler(40, 20, 360))
                        .AddLast("zlibDecoder", ZlibCodecFactory.NewZlibDecoder(ZlibWrapper.Gzip))
                        .AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 6, 4, 10, 0))
                        .AddLast("decoder", packetHeaderDecoder)
                        .AddLast("zlibEncoder", ZlibCodecFactory.NewZlibEncoder(ZlibWrapper.Gzip))
                        .AddLast("encoder", packetHeaderEncoder)
                        .AddLast("serviceBusiness", serviceBusinessHandler);
                }));
            var serverChannel = await serverBootstrap.BindAsync(networkSetting.Port);
            _serverChannel = serverChannel;
        }
        catch
        {
            await ReleaseLinkAsync();
            throw;
        }
    }

    public async Task ReleaseLinkAsync()
    {
        if (_serverChannel is { Active: true })
        {
            await _serverChannel.CloseAsync();
            _serverChannel = null;
        }
        
        foreach (var channel in AllChannels.Values)
        {
           channel.Dispose();
        }
        
        if (_bossGroup != null)
        {
           _ = Task.Run(async () =>
            {
                await _bossGroup.ShutdownGracefullyAsync();
            });
            _bossGroup = null;
        }
        if (_workerGroup != null)
        {
            _ = Task.Run(async () =>
            {
                await _workerGroup.ShutdownGracefullyAsync();
            });
            _workerGroup = null;
        }
    }
}