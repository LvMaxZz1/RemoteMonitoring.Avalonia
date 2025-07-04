using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using ReactiveUI;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Models;
using RemoteMonitoring.Core.Services.Networks;
using RemoteMonitoring.Core.Services.Networks.Base;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;
using RemoteMonitoring.Core.Utils;
using RemoteMonitoringService.Base.MessageBusModels;
using RemoteMonitoringService.Base.Network;

namespace RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

public partial class ContentPanelViewModel : ViewModelBase
{
    #region observableProperty

    private ObservableCollection<HostInfo> HostInfos { get; set; } = [];

    public ObservableCollection<HostInfo> FilterHostInfos { get; private set; } = [];
    

    [ObservableProperty] 
    private string? _searchHostName;

    [ObservableProperty]
    private HostInfo _selectedHostInfo;

    #endregion observableProperty

    #region privateField

    private readonly ISystemInfoService _systemInfoService;
    private readonly ChannelCloseSwitch _channelCloseSwitch;

    #endregion

    public AsyncRelayCommand DeleteSelectHostCommand { get; set; }

    public ContentPanelViewModel(ISystemInfoService systemInfoService, ChannelCloseSwitch channelCloseSwitch)
    {
        _systemInfoService = systemInfoService;
        _channelCloseSwitch = channelCloseSwitch;
        HostInfos.CollectionChanged += (s, e) => RefreshFilter();
        DeleteSelectHostCommand = new AsyncRelayCommand(DeleteSelectHostCommandAsync);
        
        MessageBusUtil.ListenMessage<AddHostInfoBusModel>(RxApp.MainThreadScheduler, ManualAddHostInfo, MessageBusContract.MessageBusService);
    }

    [Description("添加主机信息")]
    public async void AddHostInfo(ClientLinkChannel clientLinkChannel)
    {
        try
        {
            await UiThreadUtil.UiThreadInvokeAsync(async () =>
            {
                if (HostInfos.Any(x => x.IP == clientLinkChannel.Channel.RemoteAddress.ToString()))
                    return;
                HostInfos.Add(new HostInfo
                {
                    MachineName = Environment.MachineName,
                    IP = clientLinkChannel.Channel.RemoteAddress.ToString() ?? string.Empty,
                    LoginTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    MachineType = clientLinkChannel.LinkType switch
                    {
                        MachineLinkType.Client => "客户端",
                        MachineLinkType.Console => "控制端",
                        _ => "Unknown"
                    },
                    Address = "中山",
                    OsVersion = await _systemInfoService.GetOsVersionStrAsync(new OSInfo()),
                    Channel = clientLinkChannel.Channel
                });
            });
        }
        catch
        {
            // 错误处理
        }
    }

    [Description("移除主机信息")]
    public async void RemoveHostInfo(ClientLinkChannel? clientLinkChannel)
    {
        try
        {
            if (clientLinkChannel == null)
                return;

            UiThreadUtil.UiThreadInvoke(() =>
            {
                var hostInfo = HostInfos.FirstOrDefault(x => x.IP == clientLinkChannel.Channel?.RemoteAddress.ToString());
                if (hostInfo == null)
                    return;
                HostInfos.Remove(hostInfo);
            });
        }
        catch
        {
            // 错误处理
        }
    }
}