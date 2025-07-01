using System;
using System.Collections.Generic;
using System.Linq;
using RemoteMonitoring.Core.DependencyInjection.Base;

namespace RemoteMonitoringService.Base.Network;

[AsType(LifetimeEnum.SingleInstance)]
public class ChannelCloseSwitch
{
    public List<ChannelCloseState> ChannelCloseStates { get; set; } = [];
    
    public ChannelCloseState GetChannelCloseState(Guid machineId)
    {
        return ChannelCloseStates.First(x => x.MachineId == machineId);
    }

    public void ModifyChannelCloseState(Guid machineId, bool isClose = true)
    {
        var state = GetChannelCloseState(machineId);
        state.IsClose = isClose;
    }
}

public class ChannelCloseState
{
    public bool IsClose { get; set; }

    public Guid MachineId { get; set; }

    public ChannelCloseState(bool isClose, Guid machineId)
    {
        IsClose = isClose;
        MachineId = machineId;
    }
}