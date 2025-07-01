using System.ComponentModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using RemoteMonitoring.Core.Base;
using RemoteMonitoringService.Base.Network;
using RemoteMonitoringService.ViewModels.Base;
using RemoteMonitoringService.ViewModels.SupervisorySingle.Components;
using RemoteMonitoringService.Views.SupervisorySingle.Components;

namespace RemoteMonitoringService.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IActivatableViewModel
{
    #region observableProperty

    [ObservableProperty] private UserControl _infoControl;

    [ObservableProperty] private LeftPanel _leftPanel;

    [ObservableProperty] private ContentPanel _contentPanel;

    [ObservableProperty] private HomepagePanel _homepagePanel;

    [ObservableProperty] private NavigationPanel _navigationPanel;

    [ObservableProperty] private SystemSettingsPanel _systemSettingsPanel;

    #endregion

    #region privateField

    private readonly IServiceNetworkService _serviceNetworkService;
    private readonly IFileSecureStorage _fileSecureStorage;

    #endregion

    public ViewModelActivator Activator { get; } = new();

    public MainWindowViewModel(
        IServiceNetworkService serviceNetworkService, LeftPanel leftPanel,
        ContentPanel contentPanel, HomepagePanel homepagePanel,
        NavigationPanel navigationPanel, SystemSettingsPanel systemSettingsPanel,
        IFileSecureStorage fileSecureStorage, AvatarResourcesStorage avatarResourcesStorage)
    {
        _serviceNetworkService = serviceNetworkService;
        _fileSecureStorage = fileSecureStorage;
        SystemSettingsPanel = systemSettingsPanel;
        ContentPanel = contentPanel;
        HomepagePanel = homepagePanel;
        NavigationPanel = navigationPanel;
        LeftPanel = leftPanel;
        InfoControl = homepagePanel;

        InitPanelIconResources(fileSecureStorage, avatarResourcesStorage);

        if (LeftPanel.ViewModel is INotifyPropertyChanged npc)
        {
            npc.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(LeftPanelViewModel.CurrentLeftSideMenuItem))
                {
                    var menu = LeftPanel.ViewModel.CurrentLeftSideMenuItem;
                    OnMenuChanged(menu);
                }
            };
        }

        _ = StartCommand();
    }



    public async void OnClose()
    {
        await StopCommand();
    }
}