using System.Net.Http.Headers;

namespace RemoteMonitoring.Core.Services.Refits.DeepSeekAi;

public class DeepSeekAuthorizationHandler(RefitSetting refitSetting) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refitSetting.DeepSeekAi.ApiKey);
        return await base.SendAsync(request, cancellationToken);
    }
}