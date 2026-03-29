using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Quack.sh.Views;
using Quack.sh.Models;
using WebViewControl;

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
        
        _bridge = new TerminalBridge();
        _bridge.OnInput = (input) => _sshService.SendInput(input);
        _bridge.OnResize = (cols, rows) => _sshService.Resize(cols, rows);
        TerminalWebView.RegisterJavascriptObject("terminalBridge", _bridge);

        
        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            Config config = JsonSerializer.Deserialize<Config>(json);
            
            foreach (var connection in config.Connections)
            {
                AddConnection(connection);
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

    public void AddConnection(Connections connection)
    {
        var button = new Button
        {
            Content = connection.Name,
            Width = 200,
            Tag = connection
        };
        
        button.Click += ConnectToServer_OnClick;
        
        
        ConnectionsList.Children.Add(button);
    }

    private SshService _sshService = new SshService();
    public async void ConnectToServer_OnClick(object? sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        _currentConnection = button.Tag as Connections;
    
        var htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "xTerm.js", "terminal.html");
        TerminalWebView.Address = $"file://{htmlPath}";
    
        await System.Threading.Tasks.Task.Delay(1000);
    
        if (_currentConnection != null)
            ConnectSSH(_currentConnection);
    }
    
    private Connections? _currentConnection;

    private void OnWebViewReady(object? sender, RoutedEventArgs e)
    {
        TerminalWebView.Loaded -= OnWebViewReady;

        System.Threading.Tasks.Task.Delay(600).ContinueWith(_ =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (_currentConnection != null)
                    ConnectSSH(_currentConnection);
            });
        });
    }
    
    private TerminalBridge _bridge = new TerminalBridge();
    private void ConnectSSH(Connections connection)
    {
        Console.WriteLine($"ConnectSSH called: {connection.Host}");
    
        _sshService.Connect(connection.Host, connection.Port, connection.Username, connection.Password, (output) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                var safe = output.Replace("`", "\\`").Replace("\\", "\\\\");
                TerminalWebView.ExecuteScript($"term.write(`{safe}`)");
            });
        }, _bridge.Cols, _bridge.Rows);
    }
}