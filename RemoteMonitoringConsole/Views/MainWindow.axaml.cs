using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoringConsole.ViewModels;

namespace RemoteMonitoringConsole.Views;

[AsViewModelType(LifetimeEnum.SingleInstance, typeof(MainWindowViewModel))]
public partial class MainWindow : BaseWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        #if DEBUG
        this.AttachDevTools();
        #endif
        
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void Show()
    {
        Id = Guid.Parse("483808da-789f-4b9e-b76e-78f1e0a098cb");
        base.Show();
    }

    protected override async void OnClosed(EventArgs e)
    {
        if (DataContext is MainWindowViewModel controlMainWindowViewModel)
        {
            await controlMainWindowViewModel.MonitoringBoardPanelView.ViewModel.MonitorControlAsync();
        }
        await ViewModel.StopCommand();
        ViewModel.CloseApplication();
        base.OnClosed(e);
    }
}