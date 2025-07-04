using System;
using System.ComponentModel;
using System.Timers;
using LibreHardwareMonitor.Hardware;
using RemoteMonitoringConsole.Base.MessageBusModels;

namespace RemoteMonitoringConsole.ViewModels.SupervisorySingle.Components;

public partial class EquipmentInfoPanelViewModel
{
    [Description("初始化硬件监视器")]
    private void InitHardwareMonitor()
    {
        _computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = true,
            IsStorageEnabled = true,
            IsBatteryEnabled = true
        };
        _computer.Open();

        _hardwareTimer = new Timer(2000); // 2秒刷新一次
        _hardwareTimer.Elapsed += (s, e) => UpdateHardwareInfo();
        _hardwareTimer.Start();
    }

    [Description("更新硬件信息")]
    private void UpdateHardwareInfo()
    {
        foreach (var hardware in _computer.Hardware)
        {
            hardware.Update();
            switch (hardware.HardwareType)
            {
                case HardwareType.Cpu:
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load && sensor.Name == "CPU Total")
                            CpuLoad = sensor.Value ?? 0;
                    }

                    break;
                case HardwareType.Memory:
                    float? used = null, total = null;
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Data && sensor.Name.Contains("Memory Used"))
                            used = sensor.Value;
                        if (sensor.SensorType == SensorType.Load && sensor.Name.Equals("Memory"))
                            total = sensor.Value;
                    }

                    if (used.HasValue) MemoryUsedGb = used.Value;
                    if (total.HasValue) MemoryTotalGb = total.Value;
                    break;
                case HardwareType.GpuNvidia:
                case HardwareType.GpuAmd:
                case HardwareType.GpuIntel:
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("Core"))
                            GpuLoad = sensor.Value ?? 0;
                    }

                    break;
                case HardwareType.Storage:
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Data && sensor.Name.Contains("Used"))
                            DiskUsedGb = sensor.Value ?? 0;
                        if (sensor.SensorType == SensorType.Data && sensor.Name.Contains("Total"))
                            DiskTotalGb = sensor.Value ?? 0;
                    }

                    break;
                case HardwareType.Motherboard:
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Fan)
                            FanRpm = sensor.Value ?? 0;
                    }

                    break;
                case HardwareType.Battery:
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Power)
                            PowerUsageWatt = sensor.Value ?? 0;
                    }

                    break;
            }
        }
    }

    private void FillTerminalLog(TerminalCommandInputBusModel inputBusModel)
    {
        TerminalLogs.Add(new TerminalLog
        {
            Input = "执行命令： "+inputBusModel.Input,
            Time = DateTime.Now
        });
    }
}