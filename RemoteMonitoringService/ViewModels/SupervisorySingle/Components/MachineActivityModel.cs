using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using RemoteMonitoring.Core.Base;

namespace RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

public partial class MachineActivityModel : ViewModelBase
{
    [ObservableProperty]
    private string _iconData;

    [ObservableProperty]
    private string _iconFill;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private string _timeAgo;

    public DateTime Timestamp { get; set; }

    [Description("更新时间间隔")]
    public void UpdateTimeAgo()
    {
        var timeSpan = DateTime.Now - Timestamp;
        if (timeSpan.TotalMinutes < 1)
        {
            TimeAgo = "刚刚";
        }
        else if (timeSpan.TotalHours < 1)
        {
            TimeAgo = $"{(int)timeSpan.TotalMinutes}分钟前";
        }
        else if (timeSpan.TotalDays < 1)
        {
            TimeAgo = $"{(int)timeSpan.TotalHours}小时前";
        }
        else if (timeSpan.TotalDays < 30)
        {
            TimeAgo = $"{(int)timeSpan.TotalDays}天前";
        }
        else
        {
            TimeAgo = Timestamp.ToString("yyyy-MM-dd HH:mm");
        }
    }
}