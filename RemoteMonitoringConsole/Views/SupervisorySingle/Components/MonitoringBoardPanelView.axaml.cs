using Avalonia.Controls;
using Avalonia.Interactivity;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoringConsole.ViewModels.SupervisorySingle.Components;

namespace RemoteMonitoringConsole.Views.SupervisorySingle.Components;

[AsViewModelType(LifetimeEnum.SingleInstance, typeof(MonitoringBoardPanelViewModel))]
public partial class MonitoringBoardPanelView : BaseUserControl<MonitoringBoardPanelViewModel>
{
    public MonitoringBoardPanelView()
    {
        InitializeComponent();
    }
    
    private async void MonitorStart_OnClick(object? sender, RoutedEventArgs e)
    {
        var isControlStart = false;
        if (DataContext is MonitoringBoardPanelViewModel monitoringBoardPanelViewModel)
        {
            isControlStart = await monitoringBoardPanelViewModel.MonitorControlAsync();
        }
        
        MonitorStart = this.FindControl<Button>("MonitorStart");
        if (MonitorStart == null)
        {
            return;
        }
    
        switch (isControlStart)
        {
            case true:
                MonitorStart.Content = "停止监控";
                MonitorStart.Classes.Set("Primary", false);
                MonitorStart.Classes.Set("Danger", true);
                break;
            case false:
                MonitorStart.Content = "开始监控";
                MonitorStart.Classes.Set("Danger", false);
                MonitorStart.Classes.Set("Primary", true);
                break;
        }
    }
}