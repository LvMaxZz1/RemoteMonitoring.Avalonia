namespace RemoteMonitoring.Core.DependencyInjection.Base;

[AttributeUsage(AttributeTargets.Class)]
public class AsTypeAttribute : Attribute
{
    public AsTypeAttribute(LifetimeEnum lifetime, params Type[] types)
    {
        Lifetime = lifetime;
        Types = types;
    }

    public Type[] Types { get; set; }

    public LifetimeEnum Lifetime { get; set; }
}

public enum LifetimeEnum
{
    SingleInstance,
    Transient,
    Scope
}