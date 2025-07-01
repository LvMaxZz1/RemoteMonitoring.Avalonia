using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

namespace RemoteMonitoringService.Views.SupervisoryNoManaged.ChildWindow;

[AsViewModelType(LifetimeEnum.SingleInstance, typeof(NavigationPanelViewModel))]
public partial class ReportDisplayPanel : BaseDialogWindow<NavigationPanelViewModel>
{
    public ReportDisplayPanel()
    {
        InitializeComponent();
    }
}