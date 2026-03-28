using System;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Quack.sh.Views;

namespace Quack.sh;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "xTerm.js", "terminal.html");
        TerminalWebView.Address = $"file://{htmlPath}";
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