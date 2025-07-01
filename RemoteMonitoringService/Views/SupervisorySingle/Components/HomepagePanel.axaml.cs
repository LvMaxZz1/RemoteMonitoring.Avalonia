using Avalonia.Controls;
using Avalonia.Interactivity;
using RemoteMonitoring.Core.Base;
using RemoteMonitoring.Core.DependencyInjection.Base;
using RemoteMonitoringService.ViewModels.SupervisorySingle.Components;

namespace RemoteMonitoringService.Views.SupervisorySingle.Components;

[AsViewModelType(LifetimeEnum.SingleInstance, typeof(HomepagePanelViewModel))]
public partial class HomepagePanel : BaseUserControl<HomepagePanelViewModel>
{
    private Grid _statsGrid;
    private bool _isExpanded;
    private Border _onlineHostsBorder;
    private Border _offlineHostsBorder;
    private Border _alertCountBorder;
    private Button _toggleButton;

    public HomepagePanel()
    {
        InitializeComponent();
        _statsGrid = this.FindControl<Grid>("StatsGrid");
        _onlineHostsBorder = this.FindControl<Border>("OnlineHostsBorder");
        _offlineHostsBorder = this.FindControl<Border>("OfflineHostsBorder");
        _alertCountBorder = this.FindControl<Border>("AlertCountBorder");
        _toggleButton = this.FindControl<Button>("ToggleButton");
    }

    private void ToggleButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isExpanded)
        {
            CollapseStats();
            _toggleButton.Content = "展开";
        }
        else
        {
            ExpandStats();
            _toggleButton.Content = "收起";
        }
    }
}