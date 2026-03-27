using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Quack.sh.Views;

namespace Quack.sh;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    

    private void ShowPreferencesWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        var preferences = new PreferencesWindow();
        preferences.Show();
        
    }

    private void ShowNewConnectionWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        var newConnection = new NewConnectionWindow();
        newConnection.Show();
    }

    private void OpenGithub_OnClick(object? sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = "https://github.com/001Sarper/Quack.sh"
        });
    }

    private void ShowAboutWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        var aboutWindow = new AboutWindow();
        aboutWindow.Show();
        
    }
}