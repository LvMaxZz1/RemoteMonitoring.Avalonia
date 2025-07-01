using System.ComponentModel;
using Refit;
using RemoteMonitoring.Core.Services.Refits.DeepSeekAi.Messages;

namespace RemoteMonitoring.Core.Services.Refits.DeepSeekAi;

public interface IDeepSeekAiRefitService
{
    [Description("聊天")]
    [Post("/chat/completions")]
    [AcceptJsonHeader("Content-Type: application/json")]
    Task<DeepSeekChatResponse> ChatCompletionsAsync(
        [Body] DeepSeekChatRequest request);
}