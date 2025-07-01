namespace RemoteMonitoring.Core.Services.Refits.DeepSeekAi;

public class DeepSeekAiSetting
{
    public string BaseUrl { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string[] Model { get; set; } = ["deepseek-chat", "deepseek-reasoner"];
}