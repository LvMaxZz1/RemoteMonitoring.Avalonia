using RemoteMonitoring.Core.Base;

namespace RemoteMonitoringConsole.Base.MessageBusModels;

public class TerminalCommandInputBusModel : IMessageBusModel
{
    public string Input { get; set; }
}