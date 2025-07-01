using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RemoteMonitoring.Core.Base;

public interface IFileSecureStorage
{
    [Description("保存系统设置")]
    Task SaveSystemSettingsAsync(SystemSetting systemSetting);

    [Description("加载所有设置Json文件列表")]
    List<string> LoadSettingJsonFileList();

    [Description("加载指定设置Json文件")]
    SystemSetting LoadAppointSettingJsonFile(string settingName);

    [Description("加载最近记录的所有JsonFile")]
    IEnumerable<SystemSetting> LoadRecentlyRecordAllSettingJsonFile();

    [Description("获取icon文件夹下所有符合条件的文件名")]
    List<string> GetAllIconFileName();

    void DeleteFolder(string account);
}

public partial class SystemSetting : ObservableObject
{
    public SystemSetting()
    {
        AdminName = "Admin";
        AdminEmail = "Admain@gmail.com";
        AdminPhone = "10086";
        ProfilePicturePath = "光之战士.jfif";
        LastLoginTime = DateTime.UtcNow;
    }

    [ObservableProperty]
    private string _adminName ;

    [ObservableProperty]
    private string _adminEmail;

    [ObservableProperty]
    private string _adminPhone;

    [ObservableProperty]
    private string _profilePicturePath;

    [ObservableProperty]
    private bool _emailNotify;

    [ObservableProperty]
    private bool _smsNotify;

    [ObservableProperty]
    private bool _browserNotify;

    [ObservableProperty]
    private DateTime _lastLoginTime;

    public void CloneTo(SystemSetting targetTo)
    {
        targetTo.AdminName = this.AdminName;
        targetTo.AdminEmail = this.AdminEmail;
        targetTo.AdminPhone = this.AdminPhone;
        targetTo.ProfilePicturePath = this.ProfilePicturePath;
        targetTo.EmailNotify = this.EmailNotify;
        targetTo.SmsNotify = this.SmsNotify;
        targetTo.BrowserNotify = this.BrowserNotify;
        targetTo.LastLoginTime = this.LastLoginTime;
    }
}