using Avalonia;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Quack.sh;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        EnsureConfigFiles();
        
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    static void EnsureConfigFiles()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string configDir = Path.Combine(appData, "Quack.sh");
        
        Directory.CreateDirectory(configDir);
        
        
        string connectionsPath = Path.Combine(configDir, "Connections.json");
        string preferencesPath = Path.Combine(configDir, "ClientPreferences.json");


        if (!File.Exists(connectionsPath))
        {
            var defaultConnections = new {Connections = Array.Empty<object>()};
            File.WriteAllText(connectionsPath, JsonSerializer.Serialize(defaultConnections, new JsonSerializerOptions { WriteIndented = true }));
        }
        
        if (!File.Exists(preferencesPath))
        {
            var defaultPrefs = new { 
                ClientPreferences = new[] { new { Theme = "Dark", FontSize = 12 } } 
            };
            File.WriteAllText(preferencesPath, JsonSerializer.Serialize(defaultPrefs, new JsonSerializerOptions { WriteIndented = true }));
        }
        
    }
}
