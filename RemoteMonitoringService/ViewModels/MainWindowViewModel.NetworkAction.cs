using System.ComponentModel;
using System.Threading.Tasks;

namespace RemoteMonitoringService.ViewModels;

public partial class MainWindowViewModel
{
    [Description("开始服务")]
    private async Task StartCommand()
    {
        await _serviceNetworkService.InitLinkAsync();
    }

    [Description("停止服务")]
    private async Task StopCommand()
    {
        await _serviceNetworkService.ReleaseLinkAsync();
    }
}