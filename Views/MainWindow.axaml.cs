using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Quack.sh.Views;
using Quack.sh.Models;

namespace Quack.sh;

public partial class MainWindow : Window
{
    public static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    public static string configDir = Path.Combine(appData, "Quack.sh");
    public static string configPath = Path.Combine(configDir, "Connections.json");
    
    
    
    public MainWindow()
    {
        InitializeComponent();
        var htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "xTerm.js", "terminal.html");
        TerminalWebView.Address = $"file://{htmlPath}";


        
        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            Config config = JsonSerializer.Deserialize<Config>(json);
            
            foreach (var connection in config.Connections)
            {
                AddConnection(connection.Name);
            }
        }
        else
        {
            File.WriteAllText(configPath, "{}");
        }

        
        
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

    public void AddConnection(string connectionName)
    {
        var button = new Button
        {
            Content = connectionName,
            Width = 200,
        };
        
        ConnectionsList.Children.Add(button);
    }
}