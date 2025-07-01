using System.Text.Json;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Services.Networks.Base.Enums;

namespace RemoteMonitoring.Core.Services.Networks.Base.Messages;

public class NetworkCommand
{
    public CommandType CommandType { get; set; }
    
    public ScreenInfo? ScreenInfo { get; set; }
    
    public Guid ClientMachineId { get; set; }
    
    public Guid ConsoleMachineId { get; set; }
    
    public string? TerminalCommand { get; set; }
    
    public static string GenerateJson(
        CommandType clientActionCommandType, Guid clientMachineId, Guid consoleMachineId, ScreenInfo? screenInfo, string? terminalCommand = null)
    {
        return JsonSerializer.Serialize(new NetworkCommand
        {
            CommandType = clientActionCommandType,
            ClientMachineId = clientMachineId,
            ConsoleMachineId = consoleMachineId,
            ScreenInfo = screenInfo,
            TerminalCommand = terminalCommand
        });
    }
}