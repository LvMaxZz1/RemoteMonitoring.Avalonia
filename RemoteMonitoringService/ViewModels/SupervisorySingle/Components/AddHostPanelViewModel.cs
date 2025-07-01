using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;

namespace RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

public partial class AddHostPanelViewModel : ViewModelBase
{
    #region observableProperty

    [ObservableProperty]
    private List<OperatingSystemType> _operatingSystemList =  [OperatingSystemType.Windows, OperatingSystemType.Linux, OperatingSystemType.MacOs];
    
    [ObservableProperty]
    private OperatingSystemType _selectOperatingSystemType;
    
    [ObservableProperty]
    private List<MachineLinkType> _machineLinkTypeList = [MachineLinkType.Console, MachineLinkType.Client];
    
    [ObservableProperty]
    private MachineLinkType _selectMachineLinkType;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveAddHostInfoCommand))]
    private string _ipAddress;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveAddHostInfoCommand))]
    private string _address;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveAddHostInfoCommand))]
    private string _hostName;

    #endregion

    #region command

    public AsyncRelayCommand CancelAddHostInfoCommand { get; set; }
    
    public AsyncRelayCommand SaveAddHostInfoCommand { get; set; }

    #endregion
    
    public Action RequestClose;

    public AddHostPanelViewModel()
    {
        SelectMachineLinkType = MachineLinkTypeList[0];
        SelectOperatingSystemType = OperatingSystemList[0];
        SaveAddHostInfoCommand = new AsyncRelayCommand(SaveAddHostInfoCommandAsync, CanSave);
        CancelAddHostInfoCommand = new AsyncRelayCommand(CancelAddHostInfoCommandAsync);
    }
}

public enum OperatingSystemType
{
    Windows,
    Linux,
    MacOs
}