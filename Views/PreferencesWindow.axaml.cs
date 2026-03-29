using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Quack.sh.Models;

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
    }
}