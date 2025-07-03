using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;
using RemoteMonitoringConsole.Base.Network;

namespace RemoteMonitoringConsole.ViewModels.SupervisorySingle.Components;

public partial class MonitoringBoardPanelViewModel : ViewModelBase, IActivatableViewModel
{
    #region observableProperty
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(FullScreenCommand))]
    [NotifyPropertyChangedFor(nameof(IsFullScreen))]
    private bool _isControlStart;
    
    [ObservableProperty]
    private DrawingImage? _controlImage;

    [NotifyPropertyChangedFor(nameof(IsFullScreen))]
    [ObservableProperty]
    private bool _isOpenFullScreen;
    
    [ObservableProperty]
    private Power _power = Power.Off;

    #endregion observableProperty

    #region command

    public AsyncRelayCommand FullScreenCommand { get; set; }

    public AsyncRelayCommand SendLockScreenCommand { get; set; }
    public AsyncRelayCommand SendShutdownCommand { get; set; }

    public AsyncRelayCommand SendRestartCommand { get; set; }

    public AsyncRelayCommand SendShutdownApplicationCommand { get; set; }

    public AsyncRelayCommand SendLogoutCommand { get; set; }

    #endregion

    public IConsoleNetworkService ConsoleNetworkService { get; set; }

    public ViewModelActivator Activator { get; } = new();
    
    public bool IsFullScreen => IsControlStart && !IsOpenFullScreen;

    public MonitoringBoardPanelViewModel(IConsoleNetworkService consoleNetworkService)
    {
        CanClose = false;
        CanFloat = false;
        CanPin = false;
        ConsoleNetworkService = consoleNetworkService;
        InitCommand();
    }
    
    [Description("控制")]
    public async Task<bool> MonitorControlAsync(bool isApplicationClose = false)
    {
        switch (Power)
        {
            case Power.On:
                Power = Power.Off;
                IsControlStart = false;
                await SendCommandToClient(CommandType.ObtainScreen, new ScreenInfo(Power, 0, new Keybd(), new Mouse()));
                break;
            case Power.Off:
                if (!isApplicationClose)
                {
                    Power = Power.On;
                    IsControlStart = true;
                    await SendCommandToClient(CommandType.ObtainScreen, new ScreenInfo(Power, 50, new Keybd(), new Mouse()));
                }
                break;
        }
        return IsControlStart;
    }

    [Description("发送命令给客户端")]
    public async Task SendCommandToClient(CommandType commandType, ScreenInfo? screenInfo, string? terminalCommand = null)
    {
        await ConsoleNetworkService.SendCommandToClient(commandType, screenInfo, terminalCommand);
    }
}