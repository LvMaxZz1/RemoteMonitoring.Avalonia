using System;
using System.Threading.Tasks;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Utils;
using RemoteMonitoringService.Base.MessageBusModels;
using Tmds.DBus.Protocol;

namespace RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

public partial class AddHostPanelViewModel
{
    private async Task SaveAddHostInfoCommandAsync()
    {
        await Task.CompletedTask;
        MessageBusUtil.SendMessage(new AddHostInfoBusModel
        {
            MachineName = HostName,
            IP = IpAddress,
            LoginTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            Address = Address,
            OsVersion = SelectOperatingSystemType.ToString(),
            MachineType = SelectMachineLinkType.ToString()
        }, MessageBusContract.MessageBusService);
        MessageBusUtil.SendMessage(new SendLogBusModel
        {
            Text = $"已手动添加主机: {HostName}"
        }, MessageBusContract.MessageBusService);
        RequestClose();
    }

    private async Task CancelAddHostInfoCommandAsync()
    {
        await Task.CompletedTask;
        RequestClose();
    }

    private bool CanSave()
    {
        return !string.IsNullOrWhiteSpace(HostName) && !string.IsNullOrWhiteSpace(IpAddress) && !string.IsNullOrWhiteSpace(Address);
    }
}