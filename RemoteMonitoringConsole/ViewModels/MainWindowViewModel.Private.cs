using System.Threading.Tasks;
using RemoteMonitoringConsole.Base.Network;
using RemoteMonitoringConsole.Views.SupervisorySingle.Components;

namespace RemoteMonitoringConsole.ViewModels;

public partial class MainWindowViewModel
{
    private async Task StartLinkAsync()
    {
        await ConsoleNetworkService.InitLinkAsync();
    }
    
    private void InitPanel(IConsoleNetworkService consoleNetworkService, RemoteTerminalPanelView remoteTerminalPanelView,
        MonitoringBoardPanelView monitoringBoardPanelView, EquipmentInfoPanelView equipmentInfoPanelView)
    {
        RemoteTerminalPanelView = remoteTerminalPanelView;
        MonitoringBoardPanelView = monitoringBoardPanelView;
        EquipmentInfoPanelView = equipmentInfoPanelView;
        ConsoleNetworkService = consoleNetworkService;
    }
}