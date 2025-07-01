using System.Collections.Generic;
using Avalonia.Media.Imaging;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Models;
using RemoteMonitoringService.Assets.Base;
using RemoteMonitoringService.ViewModels.Base;

namespace RemoteMonitoringService.ViewModels;

public partial class MainWindowViewModel
{
    private void OnMenuChanged(LeftSideMenuItem menu)
    {
        NavigationPanel.ViewModel.CurrentLeftSideMenuItemTitle = "监控管理系统 / " + menu.Title;
        switch (menu.Title)
        {
            case "主机列表":
                InfoControl = ContentPanel;
                SystemSettingsPanel.ViewModel.Tick();
                break;
            case "设置":
                SystemSettingsPanel.ViewModel.SystemSetting.CloneTo(SystemSettingsPanel.ViewModel
                    .OriginalSystemSetting);
                InfoControl = SystemSettingsPanel;
                break;
            case "主页":
                InfoControl = HomepagePanel;
                SystemSettingsPanel.ViewModel.Tick();
                break;
            default:
                InfoControl = ContentPanel;
                SystemSettingsPanel.ViewModel.Tick();
                break;
        }
    }
    
    private void InitPanelIconResources(IFileSecureStorage fileSecureStorage, AvatarResourcesStorage avatarResourcesStorage)
    {
        var systemSetting = _fileSecureStorage.LoadAppointSettingJsonFile(FileSecureStorage.SystemSetting);
        var iconPathList = _fileSecureStorage.GetAllIconFileName();
        var bitMaps = new List<AvatarResources>();
        iconPathList.ForEach(str => bitMaps.Add(new AvatarResources
        {
            SystemSettingIcon = new Bitmap(AssestsPathHelper.IconBasePath + str),
            SystemSettingIconPath = AssestsPathHelper.IconBasePath + str
        }));
        var bitMap = new AvatarResources
        {
            SystemSettingIcon = new Bitmap(systemSetting.ProfilePicturePath),
            SystemSettingIconPath = systemSetting.ProfilePicturePath
        };
        LeftPanel.ViewModel.SystemSetting = systemSetting;
        SystemSettingsPanel.ViewModel.SystemSetting = systemSetting;
        SystemSettingsPanel.ViewModel.OriginalSystemSetting = SystemSettingsPanel.ViewModel.SystemSetting;
        SystemSettingsPanel.ViewModel.Tick();
        SystemSettingsPanel.ViewModel.FileSecureStorage = fileSecureStorage;
        
        avatarResourcesStorage.SystemSettingIcon = bitMap;
        avatarResourcesStorage.SystemSettingIcons = bitMaps;
        avatarResourcesStorage.SelectSystemSettingIcon = bitMap;
    }
}