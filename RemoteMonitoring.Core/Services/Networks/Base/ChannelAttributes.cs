using DotNetty.Common.Utilities;

namespace RemoteMonitoring.Core.Services.Networks.Base;

public static class ChannelAttributes
{
    public static readonly AttributeKey<MachineKey> MachineKey = 
        AttributeKey<MachineKey>.ValueOf(nameof(MachineKey));
}

public class MachineKey
{
    public Guid MachineId { get; set; }

    private MachineKey(Guid machineId)
    {
        MachineId = machineId;
    }
    
    public static MachineKey Create(Guid machineId)
    {
        return new MachineKey(machineId);
    }
}