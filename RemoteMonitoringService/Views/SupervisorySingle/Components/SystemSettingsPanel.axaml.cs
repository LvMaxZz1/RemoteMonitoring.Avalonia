using Avalonia.Interactivity;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoringService.ViewModels.SupervisorySingle.Components;
using RemoteMonitoringService.Views.SupervisoryNoManaged.ChildWindow;

namespace RemoteMonitoringService.Views.SupervisorySingle.Components;

[AsViewModelType(LifetimeEnum.SingleInstance, typeof(SystemSettingsPanelViewModel))]
public partial class SystemSettingsPanel : BaseUserControl<SystemSettingsPanelViewModel>
{
    public SystemSettingsPanel()
    {
        InitializeComponent();
    }

    private void ChangeTheAvatar_OnClick(object? sender, RoutedEventArgs e)
    {
        var avatarSelectWindow = new AvatarSelectPanel
        {
            DataContext = new AvatarSelectPanelViewModel
            {
                AvatarResourcesStorage = ViewModel.AvatarResourcesStorage,
                FileSecureStorage = ViewModel.FileSecureStorage,
                InitialProfilePicturePath = ViewModel.SystemSetting.ProfilePicturePath
            }
        };
        avatarSelectWindow.InitViewModelCallback();
        avatarSelectWindow.Show();
    }
}