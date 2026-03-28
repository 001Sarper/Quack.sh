using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Quack.sh.Models;

namespace Quack.sh;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }
        base.OnFrameworkInitializationCompleted();
        
        
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string configDir = Path.Combine(appData, "Quack.sh");
        string configPath = Path.Combine(configDir, "ClientPreferences.json");
        Directory.CreateDirectory(configDir);

        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            Config config = JsonSerializer.Deserialize<Config>(json);
            var preference = config.ClientPreferences[0];
            RequestedThemeVariant = preference.Theme == "Dark" ? ThemeVariant.Dark : ThemeVariant.Light;
        }
        else
        {
            RequestedThemeVariant = ThemeVariant.Dark; // Standard falls keine Config da
        }
    }
    
    
}