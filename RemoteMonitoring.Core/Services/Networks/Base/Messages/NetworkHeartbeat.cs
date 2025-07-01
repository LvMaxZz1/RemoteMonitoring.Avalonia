using System.Text.Json;

namespace RemoteMonitoring.Core.Services.Networks.Base.Messages;

public class NetworkHeartbeat
{
    public Guid HeartbeatMachineId { get; set; }
   
    private NetworkHeartbeat(Guid heartbeatMachineId)
    {
        HeartbeatMachineId = heartbeatMachineId;
    }

    private NetworkHeartbeat()
    {
       
    }
   
    public static NetworkHeartbeat Create(Guid heartbeatMachineId)
    {
        return new NetworkHeartbeat(heartbeatMachineId);
    }
   
    public static string GenerateJson(Guid heartbeatMachineId)
    {
        return JsonSerializer.Serialize(Create(heartbeatMachineId));
    }
}