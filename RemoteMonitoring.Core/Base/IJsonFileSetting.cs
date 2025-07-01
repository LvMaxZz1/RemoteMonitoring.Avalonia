namespace RemoteMonitoring.Core.Base;

public interface IJsonFileSetting : ISetting
{
    string JsonFilePath { get; }
}