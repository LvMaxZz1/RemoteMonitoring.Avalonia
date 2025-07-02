using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Threading;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using RemoteMonitoringService.Base.MessageBusModels;
using SkiaSharp;

namespace RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

public partial class HomepagePanelViewModel
{
    partial void OnOnlineHostsChanged(int value) => UpdateSeries();
    partial void OnOfflineHostsChanged(int value) => UpdateSeries();
    partial void OnAlertCountChanged(int value) => UpdateSeries();

    private void UpdateSeries()
    {
        SeriesCollection.Clear();
        if (OnlineHosts == 0 && OfflineHosts == 0 && AlertCount == 0)
        {
            SeriesCollection.Add(new PieSeries<double>
            {
                Values = [1],
                Name = "No Data",
                Fill = new SolidColorPaint(new SKColor(220, 220, 220)),
                IsVisible = true
            });
        }
        else
        {
            SeriesCollection.AddRange([
                new PieSeries<double>
                {
                    Tag = OnLine,
                    Values = [OnlineHosts],
                    Name = OnLine,
                    Fill = new SolidColorPaint(new SKColor(0, 166, 81)),
                    DataLabelsPosition = PolarLabelsPosition.Outer,
                    IsHoverable = true,
                    IsVisible = OnlineHosts > 0
                },
                new PieSeries<double>
                {
                    Tag = OffLine,
                    Values = [OfflineHosts],
                    Name = OffLine,
                    Fill = new SolidColorPaint(new SKColor(154, 23, 31)),
                    DataLabelsPosition = PolarLabelsPosition.Outer,
                    IsHoverable = true,
                    IsVisible = OfflineHosts > 0
                },
                new PieSeries<double>
                {
                    Tag = AlertLine,
                    Values = [AlertCount],
                    Name = AlertLine,
                    Fill = new SolidColorPaint(new SKColor(251, 251, 199)),
                    DataLabelsPosition = PolarLabelsPosition.Outer,
                    IsHoverable = true,
                    IsVisible = AlertCount > 0
                }
            ]);
        }
    }

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