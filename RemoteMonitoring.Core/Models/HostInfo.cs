using DotNetty.Transport.Channels;

namespace RemoteMonitoring.Core.Models;

public class HostInfo
{
    public string MachineName { get; set; }
    
    public string IP { get; set; }
    
    public string LoginTime { get; set; }

    public string Address { get; set; }

    public string OsVersion { get; set; }

    public string MachineType { get; set; }
    
    public IChannel? Channel { get; set; }
}