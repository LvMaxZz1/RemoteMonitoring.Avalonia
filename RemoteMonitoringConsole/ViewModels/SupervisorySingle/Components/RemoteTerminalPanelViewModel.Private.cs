using System.ComponentModel;
using RemoteMonitoring.Core.Utils;
using RemoteMonitoringConsole.Base.MessageBusModels;

namespace RemoteMonitoringConsole.ViewModels.SupervisorySingle.Components;

public partial class RemoteTerminalPanelViewModel
{
    [Description("填充终端输出")]
    private void FillTerminalOutput(TerminalCommandOutputBusModel terminalCommandOutputBusModel)
    {
        UiThreadUtil.UiThreadInvoke(() =>
        {
            TerminalOutput += terminalCommandOutputBusModel.Output + "\n";
        });
    }
}