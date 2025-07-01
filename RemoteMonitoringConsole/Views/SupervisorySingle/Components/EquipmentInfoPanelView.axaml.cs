using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoringConsole.ViewModels.SupervisorySingle.Components;

namespace RemoteMonitoringConsole.Views.SupervisorySingle.Components;

[AsViewModelType(LifetimeEnum.SingleInstance, typeof(EquipmentInfoPanelViewModel))]
public partial class EquipmentInfoPanelView : BaseUserControl<EquipmentInfoPanelViewModel>
{
    public EquipmentInfoPanelView()
    {
        InitializeComponent();
    }
}