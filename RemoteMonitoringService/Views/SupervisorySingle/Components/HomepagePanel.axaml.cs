using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

namespace RemoteMonitoringService.Views.SupervisorySingle.Components;

[AsViewModelType(LifetimeEnum.SingleInstance, typeof(HomepagePanelViewModel))]
public partial class HomepagePanel : BaseUserControl<HomepagePanelViewModel>
{
    private bool _isExpanded;

    public HomepagePanel()
    {
        InitializeComponent();
    }
}