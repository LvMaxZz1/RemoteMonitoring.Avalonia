using Avalonia.Controls;

namespace RemoteMonitoring.Core.Base;

public class BaseUserControl : UserControl, IGraphicalInterface
{
    public Guid Id { get; set; } = Guid.NewGuid();
}

public class BaseUserControl<T> : BaseUserControl where T : ViewModelBase
{
    public T ViewModel => (DataContext as T)!;
}