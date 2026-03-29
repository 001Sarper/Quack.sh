using System;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Org.BouncyCastle.Utilities;
using Quack.sh.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Quack.sh;

public partial class NewConnectionWindow : Window
{
    
    public static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    public static string configDir = Path.Combine(appData, "Quack.sh");
    public static string configPath = Path.Combine(configDir, "Connections.json");
    
    public NewConnectionWindow()
    {
        InitializeComponent();
    }

    private async void AddConnection_OnClick(object? sender, RoutedEventArgs e)
    {
        Config config;
        
        if (File.Exists(configPath))
        {
            string existing = File.ReadAllText(configPath);
            config = JsonSerializer.Deserialize<Config>(existing);
        }
        else
        {
            config = new Config();
        }
        
        if (NameTextBox.Text != "" && HostTextBox.Text.Contains(".") && int.TryParse(PortTextBox.Text, out int port) &&
            UserTextBox.Text != "" && PasswordTextBox.Text != "")
        {
            

            config.Connections.Add(new Connections
            {
                Name = NameTextBox.Text,
                Host = HostTextBox.Text,
                Port = port,
                Username = UserTextBox.Text,
                Password = PasswordTextBox.Text
            });
            
            string json =  JsonSerializer.Serialize(config);
            File.WriteAllText(configPath, json);
            var box = MessageBoxManager.GetMessageBoxStandard("Connection added successfully",
                "Connection added.", ButtonEnum.Ok);
            await box.ShowAsync();
            MainWindow mainWindow = new MainWindow();
            mainWindow.AddConnection(NameTextBox.Text);
        } else
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Invalid parameters",
                "Please enter valid connection parameters", ButtonEnum.Ok);
            await box.ShowAsync();
        }
        
        
    }
}