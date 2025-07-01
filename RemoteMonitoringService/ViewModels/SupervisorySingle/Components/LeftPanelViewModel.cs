using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Models;
using RemoteMonitoringService.Base.Network;
using RemoteMonitoringService.ViewModels.Base;

namespace RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

public partial class LeftPanelViewModel : ViewModelBase, IActivatableViewModel
{
    private readonly IServiceNetworkService _serviceNetworkService;

    #region observableProperty

    [ObservableProperty] 
    private List<LeftSideMenuItem> _leftSideMenuItems =
    [
        new()
        {
            Title = "主页",
            Icon =
                "M2 11.45a1 1 0 0 1 .33-.75l9-8.1a1 1 0 0 1 1.34 0l9 8.1a1 1 0 0 1 .33.75V20a2 2 0 0 1-2 2h-4a1 1 0 0 1-1-1v-4a3 3 0 0 0-6 0v4a1 1 0 0 1-1 1H4a2 2 0 0 1-2-2v-8.55Z"
        },
        new()
        {
            Title = "主机列表",
            Icon =
                "M4 6.5a2 2 0 1 0 0-4 2 2 0 0 0 0 4ZM9.5 3a1.5 1.5 0 1 0 0 3h11a1.5 1.5 0 0 0 0-3h-11ZM8 11.5c0-.83.67-1.5 1.5-1.5h11a1.5 1.5 0 0 1 0 3h-11A1.5 1.5 0 0 1 8 11.5Zm0 7c0-.83.67-1.5 1.5-1.5h11a1.5 1.5 0 0 1 0 3h-11A1.5 1.5 0 0 1 8 18.5Zm-2-7a2 2 0 1 1-4 0 2 2 0 0 1 4 0Zm-2 9a2 2 0 1 0 0-4 2 2 0 0 0 0 4Z"
        },
        new()
        {
            Title = "设置",
            Icon =
                " M8 5.07c-.8.47-1.8.46-2.6 0l-.24-.15a1.4 1.4 0 0 0-1.82.29c-.64.82-1.16 1.73-1.55 2.7a1.4 1.4 0 0 0 .66 1.71l.24.14a2.57 2.57 0 0 1 0 4.48l-.24.14a1.4 1.4 0 0 0-.66 1.72 11.1 11.1 0 0 0 1.55 2.69 1.4 1.4 0 0 0 1.82.3l.25-.15a2.57 2.57 0 0 1 3.88 2.24v.28c-.01.7.46 1.33 1.16 1.43a11 11 0 0 0 3.1 0 1.4 1.4 0 0 0 1.17-1.43v-.28a2.57 2.57 0 0 1 3.87-2.24l.25.14c.6.36 1.38.26 1.82-.29.64-.82 1.16-1.73 1.55-2.7a1.4 1.4 0 0 0-.66-1.71l-.24-.14a2.57 2.57 0 0 1 0-4.48l.24-.14a1.4 1.4 0 0 0 .66-1.72 11.08 11.08 0 0 0-1.55-2.69 1.4 1.4 0 0 0-1.82-.3l-.25.15a2.57 2.57 0 0 1-3.88-2.24v-.28c.01-.7-.46-1.33-1.16-1.43a10.99 10.99 0 0 0-3.1 0c-.7.1-1.17.72-1.17 1.43v.28C9.3 3.75 8.8 4.61 8 5.07Zm6 10.4a4 4 0 1 0-4-6.93 4 4 0 0 0 4 6.92Z"
        }
    ];

    [ObservableProperty] 
    private LeftSideMenuItem _currentLeftSideMenuItem;
    
    [ObservableProperty]
    private SystemSetting _systemSetting;
    
    [ObservableProperty]
    private AvatarResourcesStorage _avatarResourcesStorage;


    #endregion observableProperty

    public ViewModelActivator Activator { get; } = new();
    
    public LeftPanelViewModel(IServiceNetworkService serviceNetworkService, AvatarResourcesStorage avatarResourcesStorage)
    {
        AvatarResourcesStorage = avatarResourcesStorage;
        _serviceNetworkService = serviceNetworkService;
        _currentLeftSideMenuItem = _leftSideMenuItems[0];
    }
}