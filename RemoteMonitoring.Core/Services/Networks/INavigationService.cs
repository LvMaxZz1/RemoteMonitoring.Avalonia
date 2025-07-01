using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using RemoteMonitoring.Core.DependencyInjection.Base;

namespace RemoteMonitoring.Core.Services.Networks;

public interface INavigationService
{
    /// <summary>
    ///  操作并导航到指定窗口
    ///  如果当前的活动窗体是瞬态窗体，并且同时打开了多个窗体时，会关闭所有打开的相同类型窗体
    /// </summary>
    /// <param name="actionType">对当前活动窗体的操作</param>
    /// <typeparam name="T">需要导航的窗口</typeparam>
    /// /// <typeparam name="TR">当前活动窗口</typeparam>
    void ActionAndNavigateTo<T, TR>(NavigationActionType actionType) where T : Window where TR : Window;

    [Description("从容器中获取指定类型的实例")]
    T Resolve<T>() where T : class;

    /// <summary>
    /// 从容器中关闭指定窗体
    /// 只能关闭单例窗体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    void CloseWindowByIocContainer<T>() where T : Window;

    /// <summary>
    ///  从Avalonia 已打开的窗体 容器中操作当前活动窗体
    /// </summary>
    /// <typeparam name="T">需要操作的窗体</typeparam>
    void ActionActiveWindow<T>(NavigationActionType actionType) where T : Window;
    
    /// <summary>
    ///  导航到指定窗口
    /// </summary>
    /// <typeparam name="T">需要导航的窗口</typeparam>
    void NavigateTo<T>() where T : Window;

    [Description("关闭应用程序")]
    void CloseApplication();
}

[AsType(LifetimeEnum.SingleInstance, typeof(INavigationService))]
public class WindowNavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public WindowNavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void NavigateTo<T>() where T : Window
    {
        DispatcherUiThreadInvokeAsync(() =>
        {
            var newWindow = _serviceProvider.GetRequiredService<T>();
            newWindow.Show();
        });
    }

    public void ActionAndNavigateTo<T, TR>(NavigationActionType actionType) 
        where T : Window
        where TR : Window
    {
        DispatcherUiThreadInvokeAsync(() =>
        {
            // 获取当前活动窗口
            var currentWindow = GetActiveWindow<TR>();

            // 从容器获取新窗口实例
            var newWindow = _serviceProvider.GetRequiredService<T>();
            newWindow.Show();
            if (currentWindow != null)
            {
                switch (actionType)
                {
                    case NavigationActionType.Hide:
                        currentWindow.Hide();
                        break;
                    case NavigationActionType.Close:
                        currentWindow.Close();
                        break;
                }
            }
        });
    }

    public void CloseWindowByIocContainer<T>() where T : Window
    {
        DispatcherUiThreadInvokeAsync(() =>
        {
            var instance = _serviceProvider.GetRequiredService<T>();
            instance.Close();
        });
    }

    public void CloseApplication()
    {
        DispatcherUiThreadInvokeAsync(() =>
        {
            var windows = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?
                .Windows.ToList();
            if (windows == null) return;
            foreach (var window in windows)
            {
                window.Close();
            }
        });
    }

    public T Resolve<T>() where T : class
    {
       var instance = _serviceProvider.GetRequiredService<T>();
       return instance;
    }

    public void ActionActiveWindow<T>(NavigationActionType actionType) where T : Window
    {
        DispatcherUiThreadInvokeAsync(() =>
        {
            var instance = GetActiveWindow<T>();
            if (instance != null)
            {
                switch (actionType)
                {
                    case NavigationActionType.Hide:
                        instance.Hide();
                        break;
                    case NavigationActionType.Close:
                        instance.Close();
                        break;
                    
                }
            }
        });
    }

    private void DispatcherUiThreadInvokeAsync(Action action)
    {
        Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Default);
    }
    
    private static Window? GetActiveWindow<T>() where T : Window
        => (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?
            .Windows.FirstOrDefault(x=>x.IsVisible && x is T);
}

public enum NavigationActionType :byte
{
    
    Hide = 0x01,
    
    Close = 0x02
}