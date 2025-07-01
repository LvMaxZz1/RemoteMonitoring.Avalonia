using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Serilog;

namespace RemoteMonitoringService.Base.Network;

public class ZlibDiagnosticHandler : ChannelHandlerAdapter
{
    private readonly string _handlerName;
    public ZlibDiagnosticHandler(string name) => _handlerName = name;

    public override void ChannelRead(IChannelHandlerContext ctx, object msg)
    {
        if (msg is IByteBuffer buffer)
        {
            Log.Information($"Server [{_handlerName}] Input : {buffer.ReadableBytes}B");
        }
        base.ChannelRead(ctx, msg);
    }

    public override Task WriteAsync(IChannelHandlerContext context, object message)
    {
        if (message is IByteBuffer buffer)
        {
            Log.Information($"Server [{_handlerName}] Output : {buffer.ReadableBytes}B");
        }
        return base.WriteAsync(context, message);
    }
}