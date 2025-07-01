using System.Collections.Generic;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using RemoteMonitoring.Core.DependencyInjection.Base;

namespace RemoteMonitoringService.ViewModels.Base;

[AsType(LifetimeEnum.SingleInstance)]
public partial class AvatarResourcesStorage : ObservableObject
{
    #region observableProperty

    [ObservableProperty]
    private AvatarResources _systemSettingIcon;

    [ObservableProperty]
    private List<AvatarResources> _systemSettingIcons;
    
    [ObservableProperty]
    private AvatarResources _selectSystemSettingIcon;

    #endregion
}

public partial class AvatarResources : ObservableObject
{
    [ObservableProperty]
    private Bitmap _systemSettingIcon;

    [ObservableProperty]
    private string _systemSettingIconPath;
}