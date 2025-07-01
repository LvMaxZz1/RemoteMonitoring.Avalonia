using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Utils;
using RemoteMonitoringService.Base.MessageBusModels;

namespace RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

public partial class NavigationPanelViewModel : ViewModelBase, IActivatableViewModel
{
    #region observableProperty

    [ObservableProperty] 
    private string _currentLeftSideMenuItemTitle;
    
    [ObservableProperty] 
    private string _text;
    
    [ObservableProperty]
    private AiReply? _lastReport;

    [Description("控制是否可以显示点击查看")]
    [ObservableProperty] 
    private bool _isNotificationClickable;

    #endregion
    
    public ViewModelActivator Activator { get; } = new();
    
    public AsyncRelayCommand ViewAllHostsCommand { get; set; }

    public NavigationPanelViewModel()
    {
        Text = "";
        IsNotificationClickable = false;
        CurrentLeftSideMenuItemTitle = "监控管理系统 / 主页";
        MessageBusUtil.ListenMessage<SendLogBusModel>(RxApp.MainThreadScheduler, FillLog, MessageBusContract.MessageBusService);
        MessageBusUtil.ListenMessage<ReportGeneratedBusModel>(RxApp.MainThreadScheduler, HandleReportGenerated, MessageBusContract.MessageBusService);
    }
}