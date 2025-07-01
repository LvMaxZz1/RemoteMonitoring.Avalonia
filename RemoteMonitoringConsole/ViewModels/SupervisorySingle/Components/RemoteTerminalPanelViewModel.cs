using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Utils;
using RemoteMonitoringConsole.Base.MessageBusModels;
using RemoteMonitoringConsole.Base.Network;

namespace RemoteMonitoringConsole.ViewModels.SupervisorySingle.Components;

public partial class RemoteTerminalPanelViewModel : ViewModelBase
{
    #region observableProperty

    [ObservableProperty]
    private string _terminalOutput;

    [ObservableProperty]
    private string _terminalInput;

    #endregion

    #region privateField

    private readonly IConsoleNetworkService _consoleNetworkService;

    #endregion

    #region command

    public AsyncRelayCommand SendTerminalCommand { get; set; }

    #endregion
    
    public RemoteTerminalPanelViewModel(IConsoleNetworkService consoleNetworkService)
    {
        CanClose = false;
        CanFloat = false;
        CanPin = false;
        _consoleNetworkService = consoleNetworkService;
        InitCommand();
        MessageBusUtil.ListenMessage<TerminalCommandOutputBusModel>(RxApp.MainThreadScheduler, FillTerminalOutput, MessageBusContract.MessageBusConsole);
    }
}