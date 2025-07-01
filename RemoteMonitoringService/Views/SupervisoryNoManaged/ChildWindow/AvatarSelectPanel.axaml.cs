using Avalonia.Input;
using RemoteMonitoring.Core.Base;
using RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

namespace RemoteMonitoringService.Views.SupervisoryNoManaged.ChildWindow;

public partial class AvatarSelectPanel : BaseDialogWindow<AvatarSelectPanelViewModel>
{
    public AvatarSelectPanel()
    {
        InitializeComponent();
    }

    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            BeginMoveDrag(e);
    }

    public void InitViewModelCallback()
    {
        ViewModel.RequestClose = Close;
    }
}