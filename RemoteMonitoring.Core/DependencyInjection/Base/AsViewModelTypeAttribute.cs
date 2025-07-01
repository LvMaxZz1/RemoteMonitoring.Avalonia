namespace RemoteMonitoring.Core.DependencyInjection.Base;

[AttributeUsage(AttributeTargets.Class)]
public class AsViewModelTypeAttribute : Attribute
{
    public AsViewModelTypeAttribute(LifetimeEnum lifetime, Type viewModelType)
    {
        Lifetime = lifetime;
        ViewModelType = viewModelType;
    }

    public Type ViewModelType { get; set; }

    public LifetimeEnum Lifetime { get; set; }
}