using System;
using System.Collections.Generic;
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
    
    public static MainWindow Instance { get; private set; }
    
    
    public MainWindow()
    {
        Instance = this;
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
        
        
    }

   /* public async void SetBackground()
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
    }*/
    
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

    public List<Button> ButtonList = new List<Button>();
    public void AddConnection(Connections connection)
    {
        string jsonPreferences = File.ReadAllText(configPathPreferences);
        Config configPreferences = JsonSerializer.Deserialize<Config>(jsonPreferences);
        
        bool isDark = configPreferences.ClientPreferences[0].Theme == "Dark";
        var button = new Button
        {
            Content = connection.Name,
            Width = 200,
            Background = new SolidColorBrush(isDark ? Color.FromArgb(255, 22, 27, 34) : Color.FromArgb(255, 246, 248, 250), 1.0),
            Tag = connection
        };
        
        button.Click += ConnectToServer_OnClick;
        
        ButtonList.Add(button);
        ConnectionsList.Children.Add(button);
    }

    public List<WebView> TerminalList = new List<WebView>();
    
    public async void ConnectToServer_OnClick(object? sender, RoutedEventArgs e)
    {
        
        var button = sender as Button;
        
        string jsonPreferences = File.ReadAllText(configPathPreferences);
        Config configPreferences = JsonSerializer.Deserialize<Config>(jsonPreferences);
        
        SshTab tab = new SshTab
        {
            WebView = new WebView(),
            SshService = new SshService(),
            Bridge = new TerminalBridge()
        };
        
        TerminalList.Add(tab.WebView);
        _currentConnection = button.Tag as Connections;
        
        TabItem tabItem = new TabItem
        {
            Header = _currentConnection.Name,
            FontSize = 12,
            IsSelected =  true,
            Tag = tab
        };
        
        tabItem.GotFocus += SelectInput_OnClick;
        
        tabItem.Content = tab.WebView;
        
        TabController.Items.Add(tabItem);
        
        var htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "xTerm.js", "terminal.html");
        tab.WebView.Address = $"file://{htmlPath}";
        tab.Bridge = new TerminalBridge();
        tab.Bridge.OnInput = (input) => tab.SshService.SendInput(input);
        tab.Bridge.OnResize = (cols, rows) => tab.SshService.Resize(cols, rows);
        tab.WebView.RegisterJavascriptObject("terminalBridge", tab.Bridge);
        
        await Task.Delay(1000);
        tab.WebView.ExecuteScript($"term.options.fontSize = {configPreferences.ClientPreferences[0].FontSize};");
        
        bool isDark = configPreferences.ClientPreferences[0].Theme == "Dark";
        tab.WebView.ExecuteScript($"setTheme({isDark.ToString().ToLower()})");
        
        
        if (_currentConnection != null)
            ConnectSSH(_currentConnection, tab);
    }

    private void SelectInput_OnClick(object? sender, RoutedEventArgs e)
    {
        var button = sender as TabItem;
        var currentTab =  button.Tag as SshTab;
        
        currentTab.Bridge = new TerminalBridge();
        currentTab.Bridge.OnInput = (input) => currentTab.SshService.SendInput(input);
        currentTab.Bridge.OnResize = (cols, rows) => currentTab.SshService.Resize(cols, rows);
        currentTab.WebView.RegisterJavascriptObject("terminalBridge", currentTab.Bridge);
        
    }
    
    private Connections? _currentConnection;

    
    private void ConnectSSH(Connections connection, SshTab connectionTab)
    {
        connectionTab.SshService.Disconnect();
        Console.WriteLine($"ConnectSSH called: {connection.Host}");
        
    
        connectionTab.SshService.Connect(connection.Host, connection.Port, connection.Username, connection.Password, (output) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                var safe = output.Replace("`", "\\`").Replace("\\", "\\\\");
                connectionTab.WebView.ExecuteScript($"term.write(`{safe}`)");
            });
        }, connectionTab.Bridge.Cols, connectionTab.Bridge.Rows);
    }

    private void SavedHostsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        SavedHostsWindow savedHostsWindow = new SavedHostsWindow();
        savedHostsWindow.Show();
    }
}