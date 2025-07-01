using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoringConsole.ViewModels.SupervisorySingle.Components;

namespace RemoteMonitoringConsole.Base.Dock;

[AsType(LifetimeEnum.SingleInstance)]
public class DockFactory : Factory
{
    private readonly EquipmentInfoPanelViewModel _equipmentInfoPanelViewModel;
    private readonly MonitoringBoardPanelViewModel _monitoringBoardPanelViewModel;
    private readonly RemoteTerminalPanelViewModel _remoteTerminalPanelViewModel;
    private IRootDock? _rootDock;

    public DockFactory(EquipmentInfoPanelViewModel equipmentInfoPanelViewModel,
        MonitoringBoardPanelViewModel monitoringBoardPanelViewModel, RemoteTerminalPanelViewModel remoteTerminalPanelViewModel)
    {
        _equipmentInfoPanelViewModel = equipmentInfoPanelViewModel;
        _monitoringBoardPanelViewModel = monitoringBoardPanelViewModel;
        _remoteTerminalPanelViewModel = remoteTerminalPanelViewModel;
    }

    public override IRootDock CreateLayout()
    {

        var leftDock = new ProportionalDock
        {
            Title = "LeftDock",
            Proportion = 0.0,
            Orientation = Orientation.Vertical,
            ActiveDockable = null,
            CanClose = false,
            CanFloat = false,
            CanPin = false,
            VisibleDockables = CreateList<IDockable>
            (
                new ToolDock
                {
                    Id = "LeftEquipmentInfo",
                    Title = "LeftEquipmentInfo",
                    ActiveDockable = _equipmentInfoPanelViewModel,
                    Alignment = Alignment.Left,
                    Proportion = 0.15,
                    CanFloat = false,
                    CanClose = false,
                    CanPin = false
                },
                new ProportionalDockSplitter(),
                new ToolDock
                {
                    Id = "RemoteTerminal",
                    Title = "RemoteTerminal",
                    ActiveDockable = _remoteTerminalPanelViewModel,
                    Alignment = Alignment.Left,
                    Proportion = 0.45,
                    CanFloat = false,
                    CanClose = false,
                    CanPin = false
                }
            )
        };

        var monitoringBoardDock = new ToolDock
        {
            Id = "MonitoringBoard",
            Title = "MonitoringBoard",
            ActiveDockable = _monitoringBoardPanelViewModel,
            Alignment = Alignment.Right,
            Proportion = 0.35,
            CanFloat = false,
            CanClose = false,
            CanPin = false
        };
        
        var mainLayout = new ProportionalDock
        {
            VisibleDockables = CreateList<IDockable>
            (
                leftDock,
                new ProportionalDockSplitter(),
                monitoringBoardDock
            )
        };
        
        var rootDock = CreateRootDock();
        rootDock.IsCollapsable = false;
        rootDock.DefaultDockable = mainLayout;
        rootDock.VisibleDockables = CreateList<IDockable>(mainLayout);
        _rootDock = rootDock;
        return rootDock;
    }
}