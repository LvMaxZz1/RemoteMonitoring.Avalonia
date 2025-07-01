using RemoteMonitoring.Core.Base;
using RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

namespace RemoteMonitoringService.Base.MessageBusModels;

public class ReportGeneratedBusModel : IMessageBusModel
{
    public AiReply Report { get; set; }
}