using System.ComponentModel;
using Avalonia.Threading;
using RemoteMonitoringConsole.Base.MessageBusModels;

namespace RemoteMonitoringConsole.ViewModels.SupervisorySingle.Components;

public partial class RemoteTerminalPanelViewModel
{
    [Description("填充终端输出")]
    private void FillTerminalOutput(TerminalCommandOutputBusModel terminalCommandOutputBusModel)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            TerminalOutput += terminalCommandOutputBusModel.Output + "\n";
        });
    }
}