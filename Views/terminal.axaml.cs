using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Quack.sh;

public partial class terminal : Window
{
    public terminal()
    {
        InitializeComponent();
    }
    

    private void preferences_OnClick(object? sender, RoutedEventArgs e)
    {
        preferences preferences = new preferences();
        preferences.Show();
        
    }

    private void newConnection_OnClick(object? sender, RoutedEventArgs e)
    {
        new_connection newConnection = new new_connection();
        newConnection.Show();
    }

    private void openGithub_OnClick(object? sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = "https://github.com/001Sarper/Quack.sh"
        });
    }
}