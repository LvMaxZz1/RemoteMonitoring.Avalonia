using System.Text.Json;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;

namespace RemoteMonitoring.Core.Services.Networks.Base.Messages;

public class NetworkResponse
{
    public Guid ConsoleMachineId { get; set; }
    
    public byte[] Data { get; set; }
    
    public CommandType CommandType { get; set; }
    
    public static string GenerateJson(
        CommandType commandType, Guid consoleMachineId, byte[] data)
    {
        return JsonSerializer.Serialize(new NetworkResponse
        {
            ConsoleMachineId = consoleMachineId,
            Data = data,
            CommandType = commandType
        });
    }
}