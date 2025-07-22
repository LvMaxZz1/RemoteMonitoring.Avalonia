using System;
using System.IO;

namespace RemoteMonitoringService.Assets.Base;

public class AssestsPathHelper
{
    private const string Icon = nameof(Icon);
    private const string SecureStorage = nameof(SecureStorage);
    private const string Assets = nameof(Assets);
    
    private static readonly string BasePath = AppContext.BaseDirectory;
    
    public static readonly string AssetsPath = Path.Combine(BasePath, Assets);
    
    public static readonly string IconPath = Path.Combine(AssetsPath, Icon);
    
    public static readonly string SecureStoragePath = Path.Combine(BasePath, SecureStorage);
    
    public static readonly string IconBasePath = "Assets/Icon/";
}