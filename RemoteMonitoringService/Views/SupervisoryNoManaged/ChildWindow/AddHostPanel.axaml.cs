using Avalonia.Input;
using RemoteMonitoring.Core.Base;
using RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

namespace RemoteMonitoringService.Views.SupervisoryNoManaged.ChildWindow;

public partial class AddHostPanel : BaseDialogWindow<AddHostPanelViewModel>
{
    public AddHostPanel(AddHostPanelViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.RequestClose += Close;
    }
    
    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            BeginMoveDrag(e);
    }
}