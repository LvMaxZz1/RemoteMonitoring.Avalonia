using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

namespace RemoteMonitoringService.Views.SupervisorySingle.Components;

[AsViewModelType(LifetimeEnum.SingleInstance, typeof(LeftPanelViewModel))]
public partial class LeftPanel : BaseUserControl<LeftPanelViewModel>
{
    public LeftPanel()
    {
        InitializeComponent();
    }
}