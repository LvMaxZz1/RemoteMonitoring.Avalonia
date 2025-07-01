using System;
using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using Mediator.Net;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoring.Core.Services.Networks.Base;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;
using RemoteMonitoring.Core.Services.Networks.Base.SocketPackets;
using RemoteMonitoringService.Base.Network.Handlers;

namespace RemoteMonitoringService.Base.Network.DotNettys;

[AsType(LifetimeEnum.Transient)]
public class PacketHeaderDecoder(IMediator mediator) : ByteToMessageDecoder
{
    private readonly int _allowedWindow = 60;
    
    protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
    {
        try
        {
            if (input.ReadableBytes < PacketHeader.ByteLength)
            {
                context.Channel.CloseAsync();
                return;
            }

            input.MarkReaderIndex();
            // 验证魔数
            if (input.ReadShort() != PacketHeader.MagicNumber)
            {
                context.Channel.CloseAsync();
                return;
            }

            var version = input.ReadByte();
            var messageType = (MessageType)input.ReadByte();
            var reserved = (ushort)input.ReadShort();
            var dataLength = input.ReadInt();
            var checksum = (uint)input.ReadInt();
            var link = (MachineLinkType)input.ReadByte();
            var pack = (PackType)input.ReadByte();
            var timestamp = input.ReadInt();

            // 解析包头字段
            var packetHeader = PacketHeader.Create(version, messageType, dataLength, checksum, link, pack,
                timestamp);
            if (packetHeader.DataLength > 0)
            {
                if (input.ReadableBytes < dataLength)
                {
                    input.ResetReaderIndex(); // 数据不足，重置读取位置
                    return;
                }
                // 读取业务数据
                var payloadBuffer = input.ReadBytes(packetHeader.DataLength);
                byte[] payload = new byte[packetHeader.DataLength];
                payloadBuffer.ReadBytes(payload);

                var verifyPacketHeaderResponse = mediator
                    .SendAsync<VerifyPacketHeaderCommand, VerifyPacketHeaderResponse>(
                        new VerifyPacketHeaderCommand
                        {
                            PacketHeader = packetHeader,
                            Payload = payload
                        }).Result;

                if (!verifyPacketHeaderResponse.IsVerify)
                {
                    input.SafeRelease();
                }
                
                var networkVerify = new NetworkVerify(payload, packetHeader);
                output.Add(networkVerify);
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }



    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        return;
    }
}