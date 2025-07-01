using System.ComponentModel;
using Avalonia.Platform.Storage;

namespace RemoteMonitoring.Core.Base;

public interface IFileTransfer
{
    [Description("文件传输")]
    Task FileTransferAsync(IStorageItem file, CancellationToken cancellationToken = default);
}