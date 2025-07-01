using RemoteMonitoring.Core.Base;

namespace RemoteMonitoringService.Base.MessageBusModels;

public class AddHostInfoBusModel : IMessageBusModel
{
    public string MachineName { get; set; }
    
    public string IP { get; set; }
    
    public string LoginTime { get; set; }

    public string Address { get; set; }

    public string OsVersion { get; set; }

    public string MachineType { get; set; }
}