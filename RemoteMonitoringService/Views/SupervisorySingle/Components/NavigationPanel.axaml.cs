using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoringService.ViewModels.SupervisorySingle.Components;
using RemoteMonitoringService.Views.SupervisoryNoManaged.ChildWindow;

namespace RemoteMonitoringService.Views.SupervisorySingle.Components;

[AsViewModelType(LifetimeEnum.SingleInstance, typeof(NavigationPanelViewModel))]
public partial class NavigationPanel : BaseUserControl<NavigationPanelViewModel>
{
    private string _oldText = "";

    private string _lastText = "";
    
    public NavigationPanel()
    {
        InitializeComponent();
        
        this.AttachedToVisualTree += (_, __) =>
        {
            if (DataContext is INotifyPropertyChanged npc)
            {
                npc.PropertyChanged += OnViewModelPropertyChanged;
            }
        };
    }

    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Text")
        {
            var textBlock = this.FindControl<TextBlock>("AnimatedText");
            if (textBlock != null && textBlock.RenderTransform is TranslateTransform tt)
            {
                // 先让Y变为30
                tt.Y = 50;
                _lastText = ViewModel.Text;
                textBlock.Text = _oldText;
                await Task.Delay(500); // 让UI刷新
                tt.Y = 0; // 触发动画
                textBlock.Text = _lastText;
                _oldText = _lastText;
            }
        }
    }

    private void OnAnimatedTextTapped(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is { IsNotificationClickable: true, LastReport: not null })
        {
            var reportPanel = new ReportDisplayPanel
            {
                DataContext = ViewModel
            };
            reportPanel.Show();
        }
    }
}