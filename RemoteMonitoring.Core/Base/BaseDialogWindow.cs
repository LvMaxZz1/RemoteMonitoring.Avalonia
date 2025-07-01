using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace RemoteMonitoring.Core.Base;

/// <summary>
/// 对话框窗口基类，自动设置Owner, Position 为主窗体
/// </summary>
public class BaseDialogWindow : BaseWindow
{
    public BaseDialogWindow()
    {
        SetupWindow();
    }

    protected virtual void SetupWindow()
    {
        // 设置Owner为主窗口
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
        if (desktop.MainWindow != null)
        {
            Owner = desktop.MainWindow;
            Position = desktop.MainWindow.Position;
        }

        // 默认设置
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        CanResize = false;
    }
}

/// <summary>
/// 带ViewModel的对话框窗口基类
/// </summary>
public class BaseDialogWindow<T> : BaseDialogWindow where T : ViewModelBase
{
    public T ViewModel => (DataContext as T)!;
} 