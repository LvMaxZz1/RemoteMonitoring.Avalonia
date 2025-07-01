using DotNetty.Buffers;

namespace RemoteMonitoring.Core.Extensions;

public static class ByteBufferExtensions
{
    public static IByteBuffer WriteUnsignedInt(this IByteBuffer buffer, uint value)
    {
        buffer.WriteByte((byte)((value >> 24) & 0xFF));
        buffer.WriteByte((byte)((value >> 16) & 0xFF));
        buffer.WriteByte((byte)((value >> 8) & 0xFF));
        buffer.WriteByte((byte)(value & 0xFF));
        return buffer;
    }
}