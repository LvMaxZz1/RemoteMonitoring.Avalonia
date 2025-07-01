using RemoteMonitoring.Core.Base;

namespace RemoteMonitoringService.Base.MessageBusModels;

public class SendLogBusModel : IMessageBusModel
{
    public string Text { get; set; } = string.Empty;
}