using System;
using System.Threading.Tasks;

namespace RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

public partial class SystemSettingsPanelViewModel
{
    private async Task CancelModifyCommandAsync()
    {
        Tick();
        await Task.CompletedTask;
    }
    
    private async Task SaveModifyCommandAsync()
    {
        OriginalSystemSetting.SmsNotify = SmsNotify;
        OriginalSystemSetting.EmailNotify = EmailNotify;
        OriginalSystemSetting.AdminName = AdminName;
        OriginalSystemSetting.BrowserNotify = BrowserNotify;
        OriginalSystemSetting.ProfilePicturePath = AvatarResourcesStorage.SelectSystemSettingIcon.SystemSettingIconPath;
        OriginalSystemSetting.AdminEmail = AdminEmail;
        OriginalSystemSetting.AdminPhone = AdminPhone;
        OriginalSystemSetting.LastLoginTime = DateTime.Now;
        OriginalSystemSetting.CloneTo(SystemSetting);
        await FileSecureStorage.SaveSystemSettingsAsync(SystemSetting);
        await Task.CompletedTask;
    }
}