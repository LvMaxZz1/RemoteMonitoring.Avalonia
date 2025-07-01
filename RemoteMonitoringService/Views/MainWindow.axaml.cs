using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoringService.ViewModels;

namespace RemoteMonitoringService.Views;

[AsViewModelType(LifetimeEnum.SingleInstance, typeof(MainWindowViewModel))]
public partial class MainWindow : BaseWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        SetWindowPosition();
    }

    private void SetWindowPosition()
    {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
    }

    protected override void OnClosed(EventArgs e)
    {
        ViewModel.OnClose();
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var windows = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?
                .Windows.ToList();
            if (windows == null) return;
            foreach (var window in windows)
            {
                window.Close();
            }
        }, DispatcherPriority.Default);
        base.OnClosed(e);
    }
}