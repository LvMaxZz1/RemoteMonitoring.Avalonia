using System;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;

namespace RemoteMonitoringConsole.ViewModels.SupervisorySingle.Components;

public partial class RemoteTerminalPanelViewModel
{
    [Description("初始化命令")]
    private void InitCommand()
    {
        SendTerminalCommand = new AsyncRelayCommand(SendTerminalCommandAsync);
    }
    
    [Description("发送终端命令")]
    private async Task SendTerminalCommandAsync()
    {
        TerminalOutput += $"> {TerminalInput}\n";
        if (!string.IsNullOrWhiteSpace(TerminalInput))
        {
            if (TerminalInput.Equals("cls", StringComparison.OrdinalIgnoreCase) || TerminalInput.Equals("clear", StringComparison.OrdinalIgnoreCase))
            {
                TerminalOutput = $"> {TerminalInput}\n";
            }
            else
            {
                await _consoleNetworkService.SendCommandToClient(CommandType.SendTerminalCommand, null, TerminalInput);
            }
            TerminalInput = string.Empty;
        }
    }
}