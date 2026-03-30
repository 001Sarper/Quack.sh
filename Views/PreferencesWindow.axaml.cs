using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Quack.sh.Models;
using WebViewControl;

namespace Quack.sh;

public partial class PreferencesWindow : Window
{
    public static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    public static string configDir = Path.Combine(appData, "Quack.sh");
    public static string configPath = Path.Combine(configDir, "ClientPreferences.json");
    
    public static string json = File.ReadAllText(configPath);
    public static Config config = JsonSerializer.Deserialize<Config>(json);
    
    public PreferencesWindow()
    {
        InitializeComponent();
        
        var savedTheme = config.ClientPreferences[0].Theme;
        var savedFontSize = config.ClientPreferences[0].FontSize;

        ThemeSelection.IsEditable = true;
        ThemeSelection.Text = savedTheme;
        ThemeSelection.IsEditable = false;
        FontSize.Value = savedFontSize;
    }

    private void Save_OnClick(object? sender, RoutedEventArgs e)
    {
        
        if (!File.Exists(configPath))
        {
            var defaultConfig = new Config
            {
                ClientPreferences = new List<ClientPreferences>
                {
                    new ClientPreferences { Theme = "Dark", FontSize = 12 }
                }
            };
            File.WriteAllText(configPath, JsonSerializer.Serialize(defaultConfig));
        }
        
        config.ClientPreferences[0].Theme = ThemeSelection.Text;
        config.ClientPreferences[0].FontSize = (int)FontSize.Value;

        File.WriteAllText(configPath, JsonSerializer.Serialize(config));

        App.Instance.SetTheme(ThemeSelection.Text);
        if (MainWindow.Instance.TerminalList.Any() && MainWindow.Instance.ButtonList.Any())
        {
            bool isDark = ThemeSelection.Text ==  "Dark";
            
            foreach (WebView webView in MainWindow.Instance.TerminalList)
            {
                webView.ExecuteScript($"term.options.fontSize = {(int)FontSize.Value};");
                webView.ExecuteScript($"setTheme({isDark.ToString().ToLower()})");
            }
            
            foreach (Button button in MainWindow.Instance.ButtonList)
            {
                button.Background =
                    new SolidColorBrush(isDark ? Color.FromArgb(255, 22, 27, 34) : Color.FromArgb(255, 246, 248, 250),
                        1.0);
            }
        }
    }
}