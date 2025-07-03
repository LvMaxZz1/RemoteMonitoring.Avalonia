using Avalonia.Controls;
using SukiUI.Controls;

namespace RemoteMonitoring.Core.Base;

public class BaseWindow : SukiWindow, IGraphicalInterface
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public bool IsClose { get; set; }

    protected override void OnClosed(EventArgs e)
    {
        IsClose = true;
        base.OnClosed(e);
    }
    
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        IsClose = true;
        base.OnClosing(e);
    }
}

public class BaseWindow<T> : BaseWindow where T : ViewModelBase
{
    public T ViewModel => (DataContext as T)!;
}