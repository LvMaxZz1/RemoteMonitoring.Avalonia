using System;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using RemoteMonitoring.Core.Services.Networks.Base.SocketPackets;

namespace RemoteMonitoringService.Base.Network.Handlers;

public class VerifyPacketHeaderHandler : ICommandHandler<VerifyPacketHeaderCommand, VerifyPacketHeaderResponse>
{
    private readonly int _allowedWindow = 60;
    
    public async Task<VerifyPacketHeaderResponse> Handle(IReceiveContext<VerifyPacketHeaderCommand> context, CancellationToken cancellationToken)
    {
        var payload = context.Message.Payload;
        var header = context.Message.PacketHeader;
        
        var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(header.Timestamp);
        var serverTimestamp = DateTimeOffset.UtcNow;
        var secondsDifference = (serverTimestamp - dateTimeOffset).TotalSeconds;

        var response = new VerifyPacketHeaderResponse
        {
            IsVerify = true
        };
        
        // 校验和验证
        if (header.Version != PacketHeader.VersionConst || PacketHeader.GenerateChecksum(payload, header.Timestamp) != header.Checksum || secondsDifference > _allowedWindow )
        {
            response.IsVerify = false;
            return response;
        }

        await Task.CompletedTask;
        return response;
    }
}

public class VerifyPacketHeaderCommand : ICommand
{
    public PacketHeader PacketHeader { get; set; }

    public byte[] Payload { get; set; }
}

public class VerifyPacketHeaderResponse : IResponse
{
    public bool IsVerify { get; set; }
}