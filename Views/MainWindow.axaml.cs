using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Quack.sh.Views;
using Quack.sh.Models;
using WebViewControl;
using Color = Avalonia.Media.Color;

namespace Quack.sh;

public partial class MainWindow : Window
{
    public static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    public static string configDir = Path.Combine(appData, "Quack.sh");
    public static string configPath = Path.Combine(configDir, "Connections.json");
    public static string configPathPreferences = Path.Combine(configDir, "ClientPreferences.json");
    public static string jsonPreferences = File.ReadAllText(configPathPreferences);
    public static Config configPreferences = JsonSerializer.Deserialize<Config>(jsonPreferences);
    
    
    public MainWindow()
    {
        InitializeComponent();
        
        
        
        

        
        if (File.Exists(configPath) && File.Exists(configPathPreferences))
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
        SetBackground();
        
    }

    public async void SetBackground()
    {
        var htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "xTerm.js", "blankPage.html");

        TerminalWebView.Loaded += async (s, e) =>
        {
            await Task.Delay(50);
            Console.WriteLine("Loaded fired!");
            bool isDark = configPreferences.ClientPreferences[0].Theme == "Dark";
            string bgColor = isDark ? "#0D1117" : "#FFFFFF";
            TerminalWebView.ExecuteScript($"document.body.style.background = '{bgColor}'");
        };
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

    public void AddConnection(Connections connection)
    {
        bool isDark = configPreferences.ClientPreferences[0].Theme == "Dark";
        var button = new Button
        {
            Content = connection.Name,
            Width = 200,
            Background = new SolidColorBrush(isDark ? Color.FromArgb(255, 22, 27, 34) : Color.FromArgb(255, 246, 248, 250), 1.0),
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
        _bridge = new TerminalBridge();
        _bridge.OnInput = (input) => _sshService.SendInput(input);
        _bridge.OnResize = (cols, rows) => _sshService.Resize(cols, rows);
        TerminalWebView.RegisterJavascriptObject("terminalBridge", _bridge);
        
        await Task.Delay(1000);
        TerminalWebView.ExecuteScript($"term.options.fontSize = {configPreferences.ClientPreferences[0].FontSize};");
        
        bool isDark = configPreferences.ClientPreferences[0].Theme == "Dark";
        TerminalWebView.ExecuteScript($"setTheme({isDark.ToString().ToLower()})");
        
        
    
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
        _sshService.Disconnect();
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

    private void SavedHostsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        SavedHostsWindow savedHostsWindow = new SavedHostsWindow();
        savedHostsWindow.Show();
    }
}