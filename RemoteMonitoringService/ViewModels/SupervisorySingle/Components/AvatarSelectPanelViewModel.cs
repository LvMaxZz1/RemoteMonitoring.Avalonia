using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RemoteMonitoring.Core.Base;
using RemoteMonitoringService.ViewModels.Base;

namespace RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

public partial class AvatarSelectPanelViewModel : ViewModelBase
{
    #region observableProperty

    [ObservableProperty]
    private AvatarResourcesStorage _avatarResourcesStorage;

    [ObservableProperty]
    private string _initialProfilePicturePath;

    #endregion

    #region command

    public AsyncRelayCommand CancelModifyCommand { get; set; }
    
    public AsyncRelayCommand SaveModifyCommand { get; set; }

    #endregion
    
    public IFileSecureStorage FileSecureStorage { get; set; }
    
    [Description("窗体关闭回调")]
    public Action RequestClose { get; set; }
    
    public AvatarSelectPanelViewModel()
    {
        CancelModifyCommand = new AsyncRelayCommand(CancelModifyCommandAsync);
        SaveModifyCommand = new AsyncRelayCommand(SaveModifyCommandAsync);
    }
}