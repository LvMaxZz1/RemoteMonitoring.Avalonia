using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Services.Networks.Base;

namespace RemoteMonitoringService.Base.MessageBusModels;

public class MachineExitBusModel : IMessageBusModel
{
    public ClientLinkChannel ClientLinkChannel  { get; set; }
}