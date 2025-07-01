using System.ComponentModel;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using LibreHardwareMonitor.Hardware;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Models;
using RemoteMonitoring.Core.Services.Networks;

namespace RemoteMonitoringConsole.ViewModels.SupervisorySingle.Components;

public partial class EquipmentInfoPanelViewModel : ViewModelBase
{
    #region observableProperty

    [Description("CPU 当前负载百分比（0-100）")]
    [ObservableProperty]
    private float _cpuLoad;
    
    [Description("当前已用内存（单位：GB）")]
    [ObservableProperty]
    private float _memoryUsedGb;
    
    [Description("内存总容量（单位：GB）")]
    [ObservableProperty]
    private float _memoryTotalGb;
    
    [Description("GPU 当前负载百分比（0-100）")]
    [ObservableProperty]
    private float _gpuLoad;
    
    [Description("当前已用磁盘空间（单位：GB）")]
    [ObservableProperty]
    private float _diskUsedGb;
    
    [Description("磁盘总容量（单位：GB）")]
    [ObservableProperty]
    private float _diskTotalGb;
    
    [Description("风扇当前转速（单位：RPM，每分钟转数）")]
    [ObservableProperty]
    private float _fanRpm;
    
    [Description("当前功耗（单位：瓦特，W）")]
    [ObservableProperty]
    private float _powerUsageWatt;

    [Description("操作系统")]
    [ObservableProperty]
    private string _operatingSystem;

    #endregion

    #region privateField

    private readonly ISystemInfoService _systemInfoService;
    
    private Computer _computer;
    
    private Timer _hardwareTimer;
    
    #endregion

    public EquipmentInfoPanelViewModel(
        ISystemInfoService systemInfoService)
    {
        CanClose = false;
        CanFloat = false;
        CanPin = false;
        
        _systemInfoService = systemInfoService;
        OperatingSystem = _systemInfoService.GetOsVersionStrAsync(new OSInfo()).Result;
        InitHardwareMonitor();
    }
}