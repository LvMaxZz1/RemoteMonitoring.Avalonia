using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Controls;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Services.Networks;
using RemoteMonitoringConsole.Base.Dock;
using RemoteMonitoringConsole.Base.Network;
using RemoteMonitoringConsole.Views.SupervisorySingle.Components;

namespace RemoteMonitoringConsole.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    #region observableProperty

    [ObservableProperty]
    private RemoteTerminalPanelView _remoteTerminalPanelView;
    
    [ObservableProperty]
    private MonitoringBoardPanelView _monitoringBoardPanelView;
    
    [ObservableProperty]
    private EquipmentInfoPanelView _equipmentInfoPanelView;
    
    [ObservableProperty]
    private IRootDock? _layout;

    #endregion

    #region privateField

    private readonly INavigationService _navigationService;
    private readonly DockFactory _dockFactory;

    #endregion
    
    public IConsoleNetworkService ConsoleNetworkService { get; set; }
    
    public ICommand NewLayout { get; }

    public MainWindowViewModel(
        IConsoleNetworkService consoleNetworkService, INavigationService navigationService, 
        RemoteTerminalPanelView remoteTerminalPanelView, MonitoringBoardPanelView monitoringBoardPanelView, 
        EquipmentInfoPanelView equipmentInfoPanelView, DockFactory dockFactory)
    {
        _navigationService = navigationService;
        _dockFactory = dockFactory;
        Layout = dockFactory.CreateLayout();
        if (Layout is { })
        {
            dockFactory.InitLayout(Layout);
            if (Layout is { } root)
            {
                root.Navigate.Execute("Home");
            }
        }
        NewLayout = new RelayCommand(ResetLayout);
        InitPanel(consoleNetworkService, remoteTerminalPanelView, monitoringBoardPanelView, equipmentInfoPanelView);
        _ = StartLinkAsync();
    }

    [Description("停止服务")]
    public async Task StopCommand()
    {
        await ConsoleNetworkService.ReleaseLinkAsync();
    }

    public void CloseApplication()
    { 
        _navigationService.CloseApplication();
    }
}