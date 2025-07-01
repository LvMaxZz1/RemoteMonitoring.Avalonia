using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using RemoteMonitoring.Core.Services.Refits.DeepSeekAi;

namespace RemoteMonitoring.Core.Services.Refits;

public static class RefitExtension
{
    public static IServiceCollection AddRefit(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        services.AddScoped<DeepSeekAuthorizationHandler>();
        var refitSetting = serviceProvider.GetRequiredService<RefitSetting>();
        
        var refitSettings = new RefitSettings(new  SystemTextJsonContentSerializer(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower // Auto:role_id → RoleId
        }));

        services
            .AddRefitClient<IDeepSeekAiRefitService>(refitSettings)
            .AddHttpMessageHandler<DeepSeekAuthorizationHandler>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(refitSetting.DeepSeekAi.BaseUrl));

        return services;
    }
}