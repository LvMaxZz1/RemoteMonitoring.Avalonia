using System.Buffers.Binary;
using System.IO.Hashing;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;
using RemoteMonitoring.Core.Services.Networks.Base.SocketPackets;

namespace RemoteMonitoring.Core.Utils;

public static class NetworkByteConverter
{
    /// <summary>
    /// 计算CRC32校验值
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static uint ComputeCrc32(byte[] data)
    {
        var crc32 = new Crc32();
        crc32.Append(data);
        return crc32.GetCurrentHashAsUInt32();
    }
    
    /// <summary>
    /// 将数值类型转换为大端序字节数组
    /// </summary>
    public static byte[] GetBigEndianBytes<T>(T value) where T : struct
    {
        if (typeof(T) == typeof(ushort))
        {
            return BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((ushort)(object)value));
        }
        else if (typeof(T) == typeof(short))
        {
            return BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((short)(object)value));
        }
        else if (typeof(T) == typeof(uint))
        {
            return BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((uint)(object)value));
        }
        else if (typeof(T) == typeof(int))
        {
            return BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((int)(object)value));
        }
        else if (typeof(T) == typeof(ulong))
        {
            return BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((ulong)(object)value));
        }
        else if (typeof(T) == typeof(long))
        {
            return BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((long)(object)value));
        }
        throw new NotSupportedException($"Unsupported type: {typeof(T)}");
    }

    /// <summary>
    ///     从字节数组中解析出包头
    /// </summary>
    /// <param name="headerBytes"></param>
    /// <returns></returns>
    public static PacketHeader ParsePacketHeader(byte[] headerBytes)
    {
        return PacketHeader.Create(headerBytes[2],
            (MessageType)headerBytes[3], BinaryPrimitives.ReadInt32BigEndian(headerBytes.AsSpan(6,4)),
            BinaryPrimitives.ReadUInt32BigEndian(headerBytes.AsSpan(10,4)),
            (MachineLinkType)headerBytes[14], (PackType)headerBytes[15], BinaryPrimitives.ReadInt32BigEndian(headerBytes.AsSpan(16,4)));
    }
    
    public static ushort GetBigEndianByte(byte[] headerBytes, int offset, int length)
    {
        return  BinaryPrimitives.ReadUInt16BigEndian(headerBytes.AsSpan(offset, length));
    }
}