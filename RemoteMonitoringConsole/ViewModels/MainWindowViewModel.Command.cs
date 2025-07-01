namespace RemoteMonitoringConsole.ViewModels;

public partial class MainWindowViewModel
{
    public void ResetLayout()
    {
        if (Layout is not null)
        {
            if (Layout.Close.CanExecute(null))
            {
                Layout.Close.Execute(null);
            }
        }

        var layout = _dockFactory.CreateLayout();
        if (layout is not null)
        {
            _dockFactory.InitLayout(layout);
            Layout = layout;
        }
    }
}