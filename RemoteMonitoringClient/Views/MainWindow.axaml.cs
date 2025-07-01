using System;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoringClient.ViewModels;

namespace RemoteMonitoringClient.Views;

[AsViewModelType(LifetimeEnum.SingleInstance, typeof(MainWindowViewModel))]
public partial class MainWindow : BaseWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public override async void Show()
    {
        await App.EnsureRunAsAdmin();
        await ViewModel.StartLinkAsync();
        base.Hide();
    }

    protected override async void OnClosed(EventArgs e)
    {
        await ViewModel.StopCommandAsync();
        ViewModel.CloseApplication();
        base.OnClosed(e);
    }
}