using System.Collections.ObjectModel;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using ReactiveUI;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Services.Refits.DeepSeekAi;
using RemoteMonitoring.Core.Utils;
using RemoteMonitoringService.Base.MessageBusModels;

namespace RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

public partial class HomepagePanelViewModel : ViewModelBase, IActivatableViewModel
{
    #region observableProperty

    [ObservableProperty] 
    private int _totalHosts = 10;

    [ObservableProperty] 
    private int _onlineHosts;

    [ObservableProperty] 
    private int _offlineHosts;

    [ObservableProperty] 
    private int _alertCount;

    [ObservableProperty] 
    private AiReply _aiReply;

    [ObservableProperty] 
    private bool _canLogin;

    [ObservableProperty]
    private double _onLineHostPercentage;
    
    [ObservableProperty]
    private double _offLineHostPercentage;
    
    [ObservableProperty]
    private double _alertLineHostPercentage;

    #endregion observableProperty

    #region privateField

    private const string OnLine = nameof(OnLine);

    private const string OffLine = nameof(OffLine);

    private const string AlertLine = nameof(AlertLine);

    private const string _iconDate =
        "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-6h2v6zm0-8h-2V7h2v2z";

    private readonly Timer _updateTimer;

    private readonly LeftPanelViewModel _leftPanelViewModel;
    private readonly IDeepSeekAiRefitService _deepSeekAiRefitService;

    #endregion

    #region command

    public AsyncRelayCommand ViewAllHostsCommand { get; set; }

    public AsyncRelayCommand GenerateReportingCommand { get; set; }

    #endregion

    public ViewModelActivator Activator { get; } = new();

    public ObservableCollection<MachineActivityModel> RecentActivities { get; set; }

    public ObservableCollection<ISeries> SeriesCollection { get; set; } = [];

    public HomepagePanelViewModel(LeftPanelViewModel leftPanelViewModel, IDeepSeekAiRefitService deepSeekAiRefitService)
    {
        _leftPanelViewModel = leftPanelViewModel;
        _deepSeekAiRefitService = deepSeekAiRefitService;
        RecentActivities = [];
        OfflineHosts = TotalHosts - OnlineHosts;
        ViewAllHostsCommand = new AsyncRelayCommand(ViewAllHostsShow);
        GenerateReportingCommand = new AsyncRelayCommand(GenerateReportingCommandAAsync);

        UpdateSeries();
        UpdateOnLineHostPercentage();
        UpdateOffLineHostPercentage();
        UpdateAlertLineHostPercentage();

        MessageBusUtil.ListenMessage<MachineOnlineBusModel>(RxApp.MainThreadScheduler, MachineOnlineOnlineLog,
            MessageBusContract.MessageBusService);

        MessageBusUtil.ListenMessage<MachineExitBusModel>(RxApp.MainThreadScheduler, MachineOfflineLog,
            MessageBusContract.MessageBusService);

        // 创建定时器，每秒更新一次信息时间
        _updateTimer = new Timer(1000);
        _updateTimer.Elapsed += UpdateTimeAgo;
        _updateTimer.Start();
    }
}