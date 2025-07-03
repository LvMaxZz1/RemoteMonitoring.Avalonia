using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Services.Networks;
using RemoteMonitoringService.ViewModels.Base;

namespace RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

public partial class SystemSettingsPanelViewModel : ViewModelBase, IActivatableViewModel
{
    #region observableProperty

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SaveModifyCommand))]
    private string _adminName;
    
    [NotifyPropertyChangedFor(nameof(SaveModifyCommand))]
    [ObservableProperty] 
    private string _adminEmail;
    
    [NotifyPropertyChangedFor(nameof(SaveModifyCommand))]
    [ObservableProperty] 
    private string _adminPhone;
    
    [ObservableProperty] 
    private bool _emailNotify;
    
    [ObservableProperty] 
    private bool _smsNotify;
    
    [ObservableProperty] 
    private bool _browserNotify;

    [ObservableProperty]
    private bool _saveLoding;
    
    [ObservableProperty]
    private SystemSetting _systemSetting;
    
    [ObservableProperty]
    private AvatarResourcesStorage _avatarResourcesStorage;

    #endregion

    #region command

    public AsyncRelayCommand CancelModifyCommand { get; set; }
    
    public AsyncRelayCommand SaveModifyCommand { get; set; }

    public RelayCommand ShutdownApplicationCommand { get; set; }

    #endregion
    
    public ViewModelActivator Activator { get; } = new();

    public IFileSecureStorage FileSecureStorage { get; set; }

    public SystemSetting OriginalSystemSetting { get; set; } = new();

    private readonly INavigationService _navigationService;

    public SystemSettingsPanelViewModel(AvatarResourcesStorage avatarResourcesStorage, INavigationService navigationService)
    {
        _navigationService = navigationService;
        AvatarResourcesStorage = avatarResourcesStorage;
        CancelModifyCommand = new AsyncRelayCommand(CancelModifyCommandAsync);
        SaveModifyCommand = new AsyncRelayCommand(SaveModifyCommandAsync, CanSaveModifyCommand);

        ShutdownApplicationCommand = new RelayCommand(() => _navigationService.CloseApplication());
    }

    public void Tick()
    {
        AdminName = OriginalSystemSetting.AdminName;
        AdminEmail = OriginalSystemSetting.AdminEmail;
        AdminPhone = OriginalSystemSetting.AdminPhone;
        EmailNotify = OriginalSystemSetting.EmailNotify;
        SmsNotify = OriginalSystemSetting.SmsNotify;
        BrowserNotify = OriginalSystemSetting.BrowserNotify;
        try
        {
            // 重置头像资源的引用
            AvatarResourcesStorage.SystemSettingIcon = Enumerable.First<AvatarResources>(AvatarResourcesStorage.SystemSettingIcons, x =>
                x.SystemSettingIconPath == OriginalSystemSetting.ProfilePicturePath);
            AvatarResourcesStorage.SelectSystemSettingIcon = AvatarResourcesStorage.SystemSettingIcon;
        }
        catch
        {
            // 忽略在Left控件实例化时AvatarResourcesStorage内部成员为null的问题
        }
        OriginalSystemSetting.CloneTo(SystemSetting);
    }
}