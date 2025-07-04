using System.ComponentModel;
using Avalonia.Threading;
using RemoteMonitoring.Core.Utils;
using RemoteMonitoringService.Base.MessageBusModels;

namespace RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

public partial class NavigationPanelViewModel
{
    [Description("填充log")]
    private void FillLog(SendLogBusModel sendLogBusModel)
    {
        InitIsNotificationClickable();
        UiThreadUtil.UiThreadInvoke(() => { Text = $"{sendLogBusModel.Text}"; });
    }
    
    private void HandleReportGenerated(ReportGeneratedBusModel message)
    {
        UiThreadUtil.UiThreadInvoke(() =>
        {
            LastReport = message.Report;
            Text = "报告已生成";
            AllowNotificationClickable();
        });
    }

    private void InitIsNotificationClickable()
    {
        IsNotificationClickable = false;
    }

    private void AllowNotificationClickable()
    {
        IsNotificationClickable = true;
    }
}