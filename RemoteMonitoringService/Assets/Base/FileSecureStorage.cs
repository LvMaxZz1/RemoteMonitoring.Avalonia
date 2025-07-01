using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.DependencyInjection.Base;

namespace RemoteMonitoringService.Assets.Base;


[AsType(LifetimeEnum.Transient, typeof(IFileSecureStorage))]
public class FileSecureStorage : IFileSecureStorage
{
    /// <summary>
    ///  文件夹名称
    /// </summary>
    public const string SystemSetting = nameof(SystemSetting);

    public async Task SaveSystemSettingsAsync(SystemSetting systemSetting)
    {
        var folderPath = GetSettingFolderPath(SystemSetting);
        EnsureFileExists(folderPath);

        var filePath = GetJsonPath(SystemSetting);
        EnsureFileExists(filePath);
        var json = JsonSerializer.Serialize(systemSetting);
        await File.WriteAllTextAsync(filePath, json);
    }

    private static void EnsureFileExists(string filePath)
    {
        var fileDirectoryPath = Path.GetDirectoryName(filePath);
        if (fileDirectoryPath != null && !Directory.Exists(fileDirectoryPath))
        {
            Directory.CreateDirectory(fileDirectoryPath);
        }
    }

    public SystemSetting LoadAppointSettingJsonFile(string settingJsonFileName = SystemSetting)
    {
        var folderPath = GetSettingFolderPath(settingJsonFileName);
        var filePath = GetJsonPath(settingJsonFileName);
        if (!File.Exists(filePath))
            return null;
        var json = File.ReadAllText(filePath);
        var setting = JsonSerializer.Deserialize<SystemSetting>(json);
        if (setting == null)
        {
            setting = new SystemSetting();
        }
        return setting;
    }

    public IEnumerable<SystemSetting> LoadRecentlyRecordAllSettingJsonFile()
    {
        var list = LoadSettingJsonFileList();
        var settings = new List<SystemSetting>();
        list.ForEach(x =>
        {
            var setting = LoadAppointSettingJsonFile(x);
            if (setting != null)
                settings.Add(setting);
        });
        return settings;
    }

    public List<string> LoadSettingJsonFileList()
    {
        var secureStoragePath = AssestsPathHelper.SecureStoragePath;
        try
        {
            return LoadFileListByFolderPath(secureStoragePath);
        }
        catch
        {
            return [];
        }
    }
    
    /// <summary>
    /// 获取icon文件夹下所有图标文件名
    /// </summary>
    /// <returns>文件名列表（如：光之战士.jfif）</returns>
    public List<string> GetAllIconFileName()
    {
        var iconFolderPath = GetIconFolderPath();
        try
        {
            if (!Directory.Exists(iconFolderPath))
            {
                Directory.CreateDirectory(iconFolderPath);
                return [];
            }

            var allowedExtensions = new[] { "*.png", "*.ico", "*.jfif", "*.jpg" };
            return allowedExtensions.SelectMany(ext => Directory.GetFiles(iconFolderPath, ext))
                .Where(file => !string.IsNullOrWhiteSpace(file))
                .Select(Path.GetFileName)
                .Where(fileName => !string.IsNullOrWhiteSpace(fileName))
                .ToList()!;
        }
        catch
        {
            return [];
        }
    }

    public void DeleteFolder(string folderName) => File.Delete(GetSettingFolderPath(folderName));
    
    private static string GetSettingFolderPath(string folderName)=> Path.Combine(AssestsPathHelper.SecureStoragePath, folderName);
    
    private static string GetJsonPath(string folderName)=> Path.Combine(GetSettingFolderPath(folderName),  $"{SystemSetting}_settings.json");

    private static string GetIconFolderPath() => Path.Combine(AssestsPathHelper.IconPath);
    
    private List<string> LoadFileListByFolderPath(string path)
    {
        try
        {
            var subdirectories = Directory.GetDirectories(path);
            var list = subdirectories.ToList().Select(Path.GetFileName).ToList();
            list = list.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            return list!;
        }
        catch
        {
            return [];
        }
    }
}