using System.ComponentModel;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;
using RemoteMonitoring.Core.Utils;

namespace RemoteMonitoring.Core.Services.Networks.Base.SocketPackets;

[Description("Socket包头")]
public class PacketHeader
{
    public const ushort ByteLength = 20;

    [Description("魔数")] public const ushort MagicNumber = 0x1;

    public const byte VersionConst = 0x01;

    [Description("包头版本")] 
    public readonly byte Version = VersionConst;

    [Description("消息类型")]
    public readonly MessageType MessageType = MessageType.Command;

    [Description("预留位")]
    public readonly ushort Reserved = 0x00;

    [Description("数据长度")]
    public readonly int DataLength = 0x00;

    [Description("校验和")] 
    public uint Checksum { get; }

    public readonly MachineLinkType LinkType;

    public readonly PackType PackType = PackType.Default;

    [Description("时间戳（秒级）")] 
    public readonly int Timestamp;

    public PacketHeader(
        byte version, MessageType messageType, int dataLength, uint checksum, MachineLinkType linkType,
        PackType packType, int timestamp)
    {
        MessageType = messageType;
        DataLength = dataLength;
        Checksum = checksum;
        LinkType = linkType;
        PackType = packType;
        Timestamp = timestamp;
        Version = version;
    }

    private PacketHeader()
    {
    }

    public static PacketHeader Create(byte version, MessageType messageType, int dataLength, uint checksum,
        MachineLinkType linkType, PackType packType, int timestamp)
    {
        return new PacketHeader(version, messageType, dataLength, checksum, linkType, packType, timestamp);
    }

    public static uint GenerateChecksum(byte[] payload, int timestamp)
    {
        var timestampBytes = BitConverter.GetBytes(timestamp);
        var checksum = NetworkByteConverter.ComputeCrc32(timestampBytes.Concat(payload).ToArray());
        return checksum;
    }
    
    public static int ConvertToUnixTimestamp()
    {
        var utcNow = DateTime.UtcNow;
        var unixTimestamp = (int)(utcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        return unixTimestamp;
    }
}

public enum MessageType : byte
{
    [Description("命令")] Command = 0x01,

    [Description("响应")] Response = 0x02
}

public enum PackType : byte
{
    [Description("默认包头")] Default = 0xff,

    [Description("心跳")] Heartbeat = 0x01,
    
    [Description("分块")] Chunked = 0x02
    
}