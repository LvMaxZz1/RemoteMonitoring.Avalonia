using Avalonia.Interactivity;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoringService.ViewModels.SupervisorySingle.Components;
using RemoteMonitoringService.Views.SupervisoryNoManaged.ChildWindow;

namespace RemoteMonitoringService.Views.SupervisorySingle.Components;

[AsViewModelType(LifetimeEnum.SingleInstance, typeof(ContentPanelViewModel))]
public partial class ContentPanel : BaseUserControl<ContentPanelViewModel>
{
    public ContentPanel()
    {
        InitializeComponent();
    }

    private void AddHostsClick(object? sender, RoutedEventArgs e)
    {
        var addHostPanel = new AddHostPanel(new AddHostPanelViewModel());
        addHostPanel.Show();
    }
}