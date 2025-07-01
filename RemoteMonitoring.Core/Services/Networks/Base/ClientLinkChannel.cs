using DotNetty.Transport.Channels;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;

namespace RemoteMonitoring.Core.Services.Networks.Base;

public class ClientLinkChannel : IDisposable
{
    public MachineLinkType LinkType { get; set; }

    public IChannel? Channel { get; set; }

    public Guid MachineId { get; set; }

    public ClientLinkChannel(MachineLinkType linkType, IChannel? channel)
    {
        Channel = channel;
        LinkType = linkType;
    }

    private void ReleaseUnmanagedResources()
    {
        Channel?.CloseAsync().Wait();
        Channel = null;
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~ClientLinkChannel()
    {
        ReleaseUnmanagedResources();
    }
}