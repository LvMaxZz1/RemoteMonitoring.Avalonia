namespace RemoteMonitoring.Core.Services.Refits.DeepSeekAi.Messages;

public class DeepSeekChatRequest
{
    public string Model { get; set; } = "deepseek-chat";

    public List<DeepSeekMessage> Messages { get; set; } = [];
    
    public bool Stream { get; set; } = false;
}

public class DeepSeekMessage
{
    public string Role { get; set; } // "system" | "user" | "assistant"
    
    public string Content { get; set; }
}

public class DeepSeekChatResponse
{
    public string Id { get; set; }
    
    public string Object { get; set; }
    
    public long Created { get; set; }
    
    public string Model { get; set; }
    
    public List<DeepSeekChoice> Choices { get; set; }
    
    public DeepSeekUsage Usage { get; set; }
}

public class DeepSeekChoice
{
    public int Index { get; set; }
    
    public DeepSeekMessage Message { get; set; }
    
    public string FinishReason { get; set; }
}

public class DeepSeekUsage
{
    public int PromptTokens { get; set; }
    
    public int CompletionTokens { get; set; }
    
    public int TotalTokens { get; set; }
}