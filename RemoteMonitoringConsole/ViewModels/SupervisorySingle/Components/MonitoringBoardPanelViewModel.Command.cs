using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;
using RemoteMonitoringConsole.Views;

namespace RemoteMonitoringConsole.ViewModels.SupervisorySingle.Components;

public partial class MonitoringBoardPanelViewModel
{
    [Description("初始化命令")]
    private void InitCommand()
    {
        FullScreenCommand = new AsyncRelayCommand(FullScreenShowCommandAsync, () => IsControlStart);
        SendLockScreenCommand = new AsyncRelayCommand(SendLockScreenCommandAsync);
        SendShutdownCommand = new AsyncRelayCommand(SendShutdownCommandAsync);
        SendRestartCommand = new AsyncRelayCommand(SendRestartCommandAsync);
        SendShutdownApplicationCommand = new AsyncRelayCommand(SendShutdownApplicationCommandAsync);
        SendLogoutCommand = new AsyncRelayCommand(SendLogoutCommandAsync);
    }

    [Description("展示全屏监控画面")]
    private async Task FullScreenShowCommandAsync()
    {
        var fullScreenWindow = new FullScreenWindow
        {
            DataContext = this
        };
        fullScreenWindow.Show();
        await Task.CompletedTask;
    }

    [Description("发送锁定屏幕命令")]
    private async Task SendLockScreenCommandAsync()
    {
        await SendCommandToClient(CommandType.Lock, null);
    }

    [Description("发送关机命令")]
    private async Task SendShutdownCommandAsync()
    {
        await SendCommandToClient(CommandType.Shutdown, null);
    }

    [Description("发送重启命令")]
    private async Task SendRestartCommandAsync()
    {
        await SendCommandToClient(CommandType.Restart, null);
    }

    [Description("发送关闭应用命令")]
    public async Task SendShutdownApplicationCommandAsync()
    {
        await SendCommandToClient(CommandType.AvaloniaShutdown, null);
    }

    [Description("发送注销命令")]
    public async Task SendLogoutCommandAsync()
    {
        await SendCommandToClient(CommandType.Logout, null);
    }
}