using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.Services.Refits.DeepSeekAi;

namespace RemoteMonitoring.Core.Services.Refits;

public class RefitSetting : IJsonFileSetting
{
    public string JsonFilePath => "./Services/Refits/refit-setting.json";
    
    public DeepSeekAiSetting DeepSeekAi { get; set; } = new();
}