using RemoteMonitoring.Core.Base;

namespace RemoteMonitoringConsole.Base.MessageBusModels;

public class TerminalCommandOutputBusModel : IMessageBusModel
{
    public string Output { get; set; } = string.Empty;
}