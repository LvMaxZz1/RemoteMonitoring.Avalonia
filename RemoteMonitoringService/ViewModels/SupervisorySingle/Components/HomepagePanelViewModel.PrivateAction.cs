using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Threading;
using RemoteMonitoringService.Base.MessageBusModels;

namespace RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

public partial class HomepagePanelViewModel
{
    private void UpdateTimeAgo(object? sender, ElapsedEventArgs e)
    {
        foreach (var activity in RecentActivities)
        {
            activity.UpdateTimeAgo();
        }
    }

    private void MachineOnlineOnlineLog(MachineOnlineBusModel busModel)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            RecentActivities.Add(new MachineActivityModel
            {
                IconData = _iconDate,
                Description = $"新主机 {busModel.ClientLinkChannel.Channel.RemoteAddress} 已连接",
                Timestamp = DateTime.Now
            });
            OnlineHosts += 1;
            OfflineHosts = TotalHosts - OnlineHosts;
            if (OfflineHosts < 0)
            {
                OfflineHosts = 0;
            }
        });
    }
    
    private void MachineOfflineLog(MachineExitBusModel busModel)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            RecentActivities.Add(new MachineActivityModel
            {
                IconData = _iconDate,
                Description = $"主机 {busModel.ClientLinkChannel.Channel.RemoteAddress} 已断开连接",
                Timestamp = DateTime.Now
            });
            OnlineHosts -= 1;
            if (OnlineHosts < 0)
            {
                OnlineHosts = 0;
            }
            OfflineHosts = TotalHosts - OnlineHosts;
            if (OnlineHosts < 0)
            {
                OnlineHosts = 0;
            }
        });
    }
    
    [Description("跳转到主机列表")]
    private async Task ViewAllHostsShow()
    {
        _leftPanelViewModel.CurrentLeftSideMenuItem = _leftPanelViewModel.LeftSideMenuItems[1];
        await Task.CompletedTask;
    }

    [Description("生成Ai报告命令")]
    private async Task GenerateReportingCommandAAsync()
    {
        CanLogin = true;
        await GenerateReportingAsync();
        CanLogin = false;
    }
}