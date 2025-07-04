using Avalonia.Threading;

namespace RemoteMonitoring.Core.Utils;

public class UiThreadUtil
{
    public static async Task UiThreadInvokeAsync(Func<Task> callback)
    {
        await Dispatcher.UIThread.InvokeAsync(callback);
    }
    
    public static async Task UiThreadInvokeAsync(Func<Task> callback, DispatcherPriority priority)
    {
        await Dispatcher.UIThread.InvokeAsync(callback, priority);
    }
    
    public static void UiThreadInvoke(Action callback)
    {
        Dispatcher.UIThread.InvokeAsync(callback);
    }
    
    public static void UiThreadInvoke(Action callback, DispatcherPriority priority)
    {
        Dispatcher.UIThread.InvokeAsync(callback, priority);
    }
}