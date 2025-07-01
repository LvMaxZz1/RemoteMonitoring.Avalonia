using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoringConsole.ViewModels.SupervisorySingle.Components;

namespace RemoteMonitoringConsole.Views.SupervisorySingle.Components;

[AsViewModelType(LifetimeEnum.SingleInstance, typeof(RemoteTerminalPanelViewModel))]
public partial class RemoteTerminalPanelView : BaseUserControl<RemoteTerminalPanelViewModel>
{
    private INotifyPropertyChanged? _lastVm;
    
    public RemoteTerminalPanelView()
    {
        if (DataContext is INotifyPropertyChanged vm)
        {
            vm.PropertyChanged += ViewModel_PropertyChanged;
            _lastVm = vm;
        }

        DataContextChanged += (s, e) =>
        {
            if (_lastVm != null)
                _lastVm.PropertyChanged -= ViewModel_PropertyChanged;
            if (DataContext is INotifyPropertyChanged newVm)
            {
                newVm.PropertyChanged += ViewModel_PropertyChanged;
                _lastVm = newVm;
            }
        };
        InitializeComponent();
    }
    
    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "TerminalOutput")
        {
            TerminalScrollViewer = this.FindControl<ScrollViewer>("TerminalScrollViewer");
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                Task.Delay(10); // 10~50ms都可以
                TerminalScrollViewer?.ScrollToEnd();
            });
        }
    }
    
    private void TerminalInputBox_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (DataContext is RemoteTerminalPanelViewModel vm)
                vm.SendTerminalCommand.Execute(null);
        }
    }
}