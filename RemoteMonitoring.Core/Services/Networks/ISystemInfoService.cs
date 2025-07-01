using System.ComponentModel;
using System.Runtime.InteropServices;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoring.Core.Models;

namespace RemoteMonitoring.Core.Services.Networks;

public interface ISystemInfoService
{
    [Description("获取操作系统版本名称")]
    Task<string> GetOsVersionStrAsync(OSInfo osInfo);
}

[AsType(LifetimeEnum.Scope, typeof(ISystemInfoService))]
public class SystemInfoService(IHttpClientFactory httpClientFactory) : ISystemInfoService
{
    /// <summary>
    /// 获取操作系统版本名称
    /// </summary>
    /// <param name="osinfo"></param>
    /// <returns></returns>
    public async Task<string> GetOsVersionStrAsync(OSInfo osinfo)
    {
        string strClient = "";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            strClient ="Windows";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            strClient ="Linux";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            strClient ="macOS";
        }
        else
        {
            strClient = "Unknow OS";
        }
        await Task.CompletedTask;
        return strClient;
    }
}