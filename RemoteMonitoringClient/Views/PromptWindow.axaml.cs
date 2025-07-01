using Avalonia.Controls;
using Avalonia.Interactivity;

namespace RemoteMonitoringClient.Views;

public partial class PromptWindow : Window
{
    public PromptWindow()
    {
        InitializeComponent();
        this.DataContext = this;
    }

    private async void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        base.Close();
    }
}