using System.Threading.Tasks;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Services.Networks;
using RemoteMonitoringClient.Base.Network;

namespace RemoteMonitoringClient.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IClientNetworkService _clientNetworkService;
    private readonly INavigationService _navigationService;

    public MainWindowViewModel(IClientNetworkService clientNetworkService, INavigationService navigationService)
    {
        _clientNetworkService = clientNetworkService;
        _navigationService = navigationService;
    }

    public async Task StartLinkAsync()
    {
        await _clientNetworkService.InitLinkAsync();
    }

    public async Task StopCommandAsync()
    {
        await _clientNetworkService.ReleaseLinkAsync();
    }

    public void CloseApplication()
    {
        _navigationService.CloseApplication();
    }
}