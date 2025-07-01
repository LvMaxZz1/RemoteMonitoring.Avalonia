using Refit;

namespace RemoteMonitoring.Core.Services.Refits;

/// <summary>
/// 默认拥有Accept: application/json
/// </summary>
/// <param name="additionalHeaders">需要添加的头部</param>
[AttributeUsage(AttributeTargets.Method)]
public class AcceptJsonHeaderAttribute(params string[] additionalHeaders)
    : HeadersAttribute(CombineHeaders(additionalHeaders))
{
    private static List<string> HeadersList { get;  set; } = new();
    
    private static string[] CombineHeaders(string[] additionalHeaders)
    {
        var headers = new List<string> { "Accept: application/json" };
        if (additionalHeaders is { Length: > 0 })
        {
            headers.AddRange(additionalHeaders);
        }

        HeadersList = headers.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        return [.. HeadersList];
    }
    
    public List<string> GetHeaders() => HeadersList;
}