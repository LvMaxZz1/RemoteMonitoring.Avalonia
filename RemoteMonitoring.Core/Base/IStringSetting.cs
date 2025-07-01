namespace RemoteMonitoring.Core.Base;

public interface IStringSetting : ISetting
{
    string Value { get; }
}