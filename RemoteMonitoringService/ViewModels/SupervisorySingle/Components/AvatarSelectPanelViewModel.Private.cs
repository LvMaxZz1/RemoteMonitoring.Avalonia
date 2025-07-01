using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

public partial class AvatarSelectPanelViewModel
{
    [Description("取消修改头像")]
    private async Task CancelModifyCommandAsync()
    {
        AvatarResourcesStorage.SelectSystemSettingIcon = AvatarResourcesStorage.SystemSettingIcons.First(x=>x.SystemSettingIconPath == InitialProfilePicturePath);
        await Task.CompletedTask;
        RequestClose.Invoke();
    }
    
    [Description("确定头像")]
    private async Task SaveModifyCommandAsync()
    {
        if (AvatarResourcesStorage.SelectSystemSettingIcon != null)
        {
            AvatarResourcesStorage.SystemSettingIcon = AvatarResourcesStorage.SelectSystemSettingIcon;  
        }
        await Task.CompletedTask;
        RequestClose.Invoke();
    }
}