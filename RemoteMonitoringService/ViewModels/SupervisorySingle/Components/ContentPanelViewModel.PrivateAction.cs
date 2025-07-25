﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Models;
using RemoteMonitoring.Core.Services.Networks.Base;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;
using RemoteMonitoring.Core.Services.Networks.Base.Messages;
using RemoteMonitoring.Core.Services.Networks.Base.SocketPackets;
using RemoteMonitoring.Core.Utils;
using RemoteMonitoringService.Base.MessageBusModels;

namespace RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

public partial class ContentPanelViewModel
{
    private void ManualAddHostInfo(AddHostInfoBusModel busModel)
    {
        UiThreadUtil.UiThreadInvoke(() =>
        {
            HostInfos.Add(new HostInfo
            {
                MachineName = busModel.MachineName,
                IP = busModel.IP,
                LoginTime = busModel.LoginTime,
                MachineType = busModel.MachineType,
                Address = busModel.Address,
                OsVersion = busModel.OsVersion
            });
        });
    }
    
    partial void OnSearchHostNameChanged(string? value)
    {
        RefreshFilter();
    }

    private void RefreshFilter()
    {
        FilterHostInfos.Clear();
        if (!string.IsNullOrWhiteSpace(SearchHostName))
        {
            var filtered = HostInfos
                .Where(h => h.MachineName.Contains(SearchHostName, StringComparison.OrdinalIgnoreCase)
                            || h.IP.Contains(SearchHostName, StringComparison.OrdinalIgnoreCase)
                            || h.Address.Contains(SearchHostName, StringComparison.OrdinalIgnoreCase)
                            || h.MachineType.Contains(SearchHostName, StringComparison.OrdinalIgnoreCase))
                .ToList();
            FilterHostInfos.AddRange(filtered);
        }
        else
        {
            FilterHostInfos.AddRange(HostInfos);
        }
    }
    
    private async Task DeleteSelectHostCommandAsync()
    {
        if (SelectedHostInfo != null)
        {
            var machineName = SelectedHostInfo.MachineName;
            if (SelectedHostInfo.Channel is { Active: true })
            {
                var machineKey = SelectedHostInfo.Channel.GetAttribute(ChannelAttributes.MachineKey).Get();
                if (machineKey != null) 
                {
                    _channelCloseSwitch.ModifyChannelCloseState(machineKey.MachineId);
                    var networkCommandJson = NetworkCommand.GenerateJson(CommandType.AvaloniaShutdown,
                        Guid.Parse("6ad94c0e-9031-4423-9ee8-1cc2c3ff10c9"),
                        Guid.Parse("483808da-789f-4b9e-b76e-78f1e0a098cb"), null);
                    var networkCommandBytes = Encoding.UTF8.GetBytes(networkCommandJson);
                    var timestamp = PacketHeader.ConvertToUnixTimestamp();
                    var header = PacketHeader.Create(
                        PacketHeader.VersionConst, MessageType.Command, networkCommandBytes.Length,
                        PacketHeader.GenerateChecksum(networkCommandBytes, timestamp), MachineLinkType.Console,
                        PackType.Default,
                        timestamp);
                    await SelectedHostInfo.Channel.WriteAndFlushAsync(new NetworkVerify(networkCommandBytes, header));
                    await SelectedHostInfo.Channel.CloseAsync(); 
                }
                SelectedHostInfo.Channel = null;
            }

            HostInfos.Remove(SelectedHostInfo);
            MessageBusUtil.SendMessage(new SendLogBusModel { Text = $"已删除主机: {machineName}" }, MessageBusContract.MessageBusService);
        }
    }
}