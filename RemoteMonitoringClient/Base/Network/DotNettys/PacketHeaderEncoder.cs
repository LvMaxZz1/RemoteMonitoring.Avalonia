using System;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoring.Core.Extensions;
using RemoteMonitoring.Core.Services.Networks.Base;
using RemoteMonitoring.Core.Services.Networks.Base.SocketPackets;

namespace RemoteMonitoringClient.Base.Network.DotNettys;

[AsType(LifetimeEnum.Transient)]
public class PacketHeaderEncoder : MessageToByteEncoder<NetworkVerify> 
{
    protected override void Encode(IChannelHandlerContext context, NetworkVerify verify, IByteBuffer output)
    {
        var header = verify.PacketHeader;
        // 魔数（2字节大端）
        output.WriteUnsignedShort(PacketHeader.MagicNumber);
        // 版本（1字节）
        output.WriteByte(header.Version);
        // 消息类型（1字节）
        output.WriteByte((byte)header.MessageType);
        // 预留位（2字节大端）
        output.WriteUnsignedShort(header.Reserved);
        // 数据长度（4字节大端）
        output.WriteInt(header.DataLength);
        // 校验和（4字节大端）
        output.WriteUnsignedInt(header.Checksum);
        // 连接类型（1字节）
        output.WriteByte((byte)header.LinkType);
        // 包类型（1字节）
        output.WriteByte((byte)header.PackType);
        //时间戳
        output.WriteInt(header.Timestamp);
        // 写入业务数据
        output.WriteBytes(verify.ReceiveMessageBytes);
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
    }
}