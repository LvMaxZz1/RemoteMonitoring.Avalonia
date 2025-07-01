using Avalonia.Media;

namespace RemoteMonitoringService.Views.SupervisorySingle.Components;

public partial class HomepagePanel
{
    private void ExpandStats()
    {
        if (!_isExpanded)
        {
            var onlineTransform = _onlineHostsBorder.RenderTransform as TranslateTransform;
            var offlineTransform = _offlineHostsBorder.RenderTransform as TranslateTransform;
            var alertTransform = _alertCountBorder.RenderTransform as TranslateTransform;

            onlineTransform.X = 0;
            offlineTransform.X = 0;
            alertTransform.X = 0;

            _isExpanded = true;
        }
    }

    private void CollapseStats()
    {
        if (_isExpanded)
        {
            var onlineTransform = _onlineHostsBorder.RenderTransform as TranslateTransform;
            var offlineTransform = _offlineHostsBorder.RenderTransform as TranslateTransform;
            var alertTransform = _alertCountBorder.RenderTransform as TranslateTransform;

            onlineTransform.X = -120;
            offlineTransform.X = -240;
            alertTransform.X = -360;

            _isExpanded = false;
        }
    }
}