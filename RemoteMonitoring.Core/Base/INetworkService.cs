using System.ComponentModel;

namespace RemoteMonitoring.Core.Base;

public interface INetworkService
{
    [Description("初始化连接")]
    Task InitLinkAsync();

    [Description("释放链接")]
    Task ReleaseLinkAsync();
}