using RemoteMonitoring.Core.Services.Networks.Base.SocketPackets;

namespace RemoteMonitoring.Core.Services.Networks.Base;

public class NetworkVerify(byte[] receiveMessageBytes, PacketHeader packetHeader)
{
    public byte[] ReceiveMessageBytes { get; } = receiveMessageBytes; 
    
    public PacketHeader PacketHeader { get; } = packetHeader;
}