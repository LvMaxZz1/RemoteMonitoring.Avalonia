using System.Reflection;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoring.Core.Utils;

namespace RemoteMonitoring.Core.DependencyInjection;

public static class RegisterMarkedServices
{
    public static IServiceCollection RegisterServices(this IServiceCollection service)
    {
        var entityTypes = TypeUtil.ObtainImplementerByAttribute<AsTypeAttribute>();
        foreach (var implementationType in entityTypes)
        {
            var asTypeFlag = implementationType.GetCustomAttribute<AsTypeAttribute>()!;
            if (asTypeFlag is { Types: { Length: > 0 } })
            {
                foreach (var baseType in asTypeFlag.Types)
                    AddService(service, asTypeFlag.Lifetime, baseType, implementationType);
            }
            else
            {
                var baseTypes = implementationType.GetInterfaces();
                if (baseTypes is { Length: > 0 })
                {
                    foreach (var baseType in baseTypes)
                    {
                        AddService(service, asTypeFlag.Lifetime, baseType, implementationType);
                    }
                }

                if (implementationType.BaseType != null)
                {
                    AddService(service, asTypeFlag.Lifetime, implementationType.BaseType, implementationType);
                }

                AddService(service, asTypeFlag.Lifetime, implementationType, implementationType);
            }
        }

        return service;
    }
    
    public static IServiceCollection AddViewServices(this IServiceCollection services)
    {
        var types = TypeUtil.ObtainImplementerByAttribute<AsViewModelTypeAttribute>();
        foreach (var viewWindow in types)
        {
            var asTypeFlag = viewWindow.GetCustomAttribute<AsViewModelTypeAttribute>()!;
            AddImplementationType(services, asTypeFlag.ViewModelType, asTypeFlag.Lifetime);
            AddViewImplementationAndInitDataContext(services, viewWindow, asTypeFlag);
        }
        return services;
    }

    private static void AddViewImplementationAndInitDataContext(IServiceCollection services, Type viewWindow,
        AsViewModelTypeAttribute asTypeFlag)
    {
        services.AddSingleton(viewWindow, provider =>
        {
            var styledElement = (StyledElement)Activator.CreateInstance(viewWindow)!;
            styledElement.DataContext = provider.GetRequiredService(asTypeFlag.ViewModelType);
            return styledElement;
        });
    }

    private static void AddService(IServiceCollection services, LifetimeEnum lifetime, Type baseType,
        Type implementationType)
    {
        switch (lifetime)
        {
            case LifetimeEnum.SingleInstance:
                services.AddSingleton(baseType, implementationType);
                break;
            case LifetimeEnum.Transient:
                services.AddTransient(baseType, implementationType);
                break;
            case LifetimeEnum.Scope:
                services.AddScoped(baseType, implementationType);
                break;
        }
    }

    private static void AddImplementationType(IServiceCollection services, Type implementationType,
        LifetimeEnum lifetime)
    {
        switch (lifetime)
        {
            case LifetimeEnum.SingleInstance:
                services.AddSingleton(implementationType);
                break;
            case LifetimeEnum.Transient:
                services.AddTransient(implementationType);
                break;
            case LifetimeEnum.Scope:
                services.AddScoped(implementationType);
                break;
        }
    }
}