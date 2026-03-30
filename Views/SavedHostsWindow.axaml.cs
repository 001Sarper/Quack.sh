using System;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Quack.sh.Models;
using Tmds.DBus.Protocol;

namespace Quack.sh.Views;

public partial class SavedHostsWindow : Window
{
    public static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    public static string configDir = Path.Combine(appData, "Quack.sh");
    public static string configPath = Path.Combine(configDir, "Connections.json");
    
    public SavedHostsWindow()
    {
        InitializeComponent();
        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            Config config = JsonSerializer.Deserialize<Config>(json);
            int index = 0;
            
            foreach (var connection in config.Connections)
            {
                AddConnection(connection, index);
                index++;
            }
        }
        else
        {
            File.WriteAllText(configPath, "{}");
        }
        
    }

    public void AddConnection(Connections connection, int index)
    {
        
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        
        var textblock = new TextBlock{Text = connection.Name, VerticalAlignment =  VerticalAlignment.Center};
        var deleteButton = new Button
        {
            Content = "Delete",
            Tag = (connection, index)
        };
        var editButton = new Button
        {
            Content = "Edit",
            Tag = (connection, index)
        };
        
        
        deleteButton.Click += DeleteConnection;
        Grid.SetColumn(textblock, 0);
        Grid.SetColumn(editButton, 1);
        Grid.SetColumn(deleteButton, 2);
        
        grid.Children.Add(textblock);
        grid.Children.Add(editButton);
        grid.Children.Add(deleteButton);
        ParentPanel.Children.Add(grid);
        
        
        
    }
    
    public async void DeleteConnection(object? sender, RoutedEventArgs e)
    {
        var box = MessageBoxManager.GetMessageBoxStandard("Delete Connection",
            "Are you sure you want to delete this connection?", ButtonEnum.YesNo);
        var result = await box.ShowAsync();
        if (result == ButtonResult.Yes)
        {
            Console.WriteLine("Delete Connection called");
            var button = sender as Button;
            var (connection, index) = ((Connections, int))button.Tag;
        
            string json = File.ReadAllText(configPath);
            Config config = JsonSerializer.Deserialize<Config>(json);
        
            config.Connections.RemoveAll(c => c.Name == connection.Name);
            Console.WriteLine($"Connections left: {config.Connections.Count}");
        
            string newJson = JsonSerializer.Serialize(config);
            File.WriteAllText(configPath, newJson);
            
            
            ParentPanel.Children.RemoveAt(index);
            MainWindow.Instance.ConnectionsList.Children.RemoveAt(index + 1);

        } 
        
    }
}