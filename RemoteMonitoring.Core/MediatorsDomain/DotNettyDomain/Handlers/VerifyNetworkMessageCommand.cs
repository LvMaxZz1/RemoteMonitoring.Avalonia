using System.ComponentModel;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using RemoteMonitoring.Core.Services.Networks.Base;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;

namespace RemoteMonitoring.Core.MediatorsDomain.DotNettyDomain.Handlers;

public class VerifyNetworkMessageHandler : ICommandHandler<VerifyNetworkMessageCommand, VerifyNetworkMessageResponse>
{
    public async Task<VerifyNetworkMessageResponse> Handle(IReceiveContext<VerifyNetworkMessageCommand> context,
        CancellationToken cancellationToken)
    {
        var msg = context.Message.Msg;

        if (msg.ReceiveMessageBytes.Length == 0 || msg.PacketHeader.LinkType is not MachineLinkType.Client and not MachineLinkType.Console)
        {
            return new VerifyNetworkMessageResponse
            {
                IsVerify = false
            };
        }
        
        await Task.CompletedTask;
        return new VerifyNetworkMessageResponse
        {
            IsVerify = true
        };
    }
}

[Description("验证网络消息是否符合规格")]
public class VerifyNetworkMessageCommand : ICommand
{
    public NetworkVerify Msg { get; set; }
}

public class VerifyNetworkMessageResponse : IResponse
{
    public bool IsVerify { get; set; }
}