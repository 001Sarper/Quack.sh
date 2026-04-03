using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Microsoft.AspNetCore.DataProtection;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Quack.sh.Models;

namespace Quack.sh.Views;

public partial class ManageHostsWindow : Window
{
    public static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    public static string configDir = Path.Combine(appData, "Quack.sh");
    public static string configPath = Path.Combine(configDir, "Connections.json");
    
    public ManageHostsWindow()
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
    
    private void AddConnection(Connections connection, int index)
    {
        
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        
        var textblock = new TextBlock{Text = connection.Name, VerticalAlignment = VerticalAlignment.Center};
        var deleteImage = new Image { Source = new Bitmap(AssetLoader.Open(new Uri("avares://Quack.sh/Assets/Images/deleteIcon.png")))};
        var editImage = new Image { Source = new Bitmap(AssetLoader.Open(new Uri("avares://Quack.sh/Assets/Images/editIcon.png")))};
        
        var deleteButton = new Button
        {
            Content = deleteImage,
            Tag = (connection, index),
            Width = 50,
            Height = 30
        };
        var editButton = new Button
        {
            Content = editImage,
            Tag = (connection, index),
            Width = 50,
            Height = 30
        };
        
        
        deleteButton.Click += DeleteConnection;
        editButton.Click += EditConnection;
        Grid.SetColumn(textblock, 0);
        Grid.SetColumn(editButton, 1);
        Grid.SetColumn(deleteButton, 2);
        
        grid.Children.Add(textblock);
        grid.Children.Add(editButton);
        grid.Children.Add(deleteButton);
        ParentPanel.Children.Add(grid);
        
        
        
    }
    
    private async void DeleteConnection(object? sender, RoutedEventArgs e)
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

    private void EditConnection(object? sender, RoutedEventArgs e)
    {
        
        var button = sender as Button;
        var (connection, index) = ((Connections, int))button.Tag;
        
        ResetInputFields();
        
        BoxPanelButton.Content = "Save changes";
        BoxPanelButton.Tag = (index, 0);
        BoxPanelButton.Click += ManageConnection_OnClick;
        NameTextBox.Text = connection.Name;
        HostTextBox.Text = connection.Host;
        PortTextBox.Text = connection.Port.ToString();
        UserTextBox.Text = connection.Username;
        if (connection.PrivateKeyUsed)
        {
            AuthSelection.SelectedIndex = 1;
            PasswordAuth.IsVisible = false;
            PrivateKeyAuth.IsVisible = true;
            PassphraseTextBox.Text = App.Instance.Protector.Unprotect(connection.Passphrase);
        }
        else
        {
            AuthSelection.SelectedIndex = 0;
            PrivateKeyAuth.IsVisible = false;
            PasswordAuth.IsVisible = true;
            PasswordTextBox.Text = App.Instance.Protector.Unprotect(connection.Password);
        }
        
        BoxPanel.IsVisible = true;
        
        
        
    }

    private void AddConnection_OnClick(object? sender, RoutedEventArgs e)
    {
        ResetInputFields();
        AuthSelection.SelectedIndex = 0;
        BoxPanelButton.Content = "Add connection";
        BoxPanelButton.Tag = (-1, 1);
        BoxPanelButton.Click += ManageConnection_OnClick;
        BoxPanel.IsVisible = true;
    }
    

    private async void ManageConnection_OnClick(object? sender, RoutedEventArgs e)
    {
        var button =  sender as Button;
        var (index, manageMode) = ((int, int))button.Tag;
        bool privKeyUsed = (AuthSelection.SelectedIndex == 1) ? true : false;
        
        string passphrase = (string.IsNullOrEmpty(PassphraseTextBox.Text)) ? "" : PassphraseTextBox.Text;
        
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
            UserTextBox.Text != "" && (privKeyUsed ? !string.IsNullOrEmpty(_privateKey) : PasswordTextBox.Text != ""))
        {
            Connections newConnection = new Connections
            {
                Name = NameTextBox.Text,
                Host = HostTextBox.Text,
                Port = port,
                Username = UserTextBox.Text,
                Password = (!privKeyUsed) ? App.Instance.Protector.Protect(PasswordTextBox.Text) : "",
                PrivateKeyUsed = privKeyUsed,
                PrivateKey = (privKeyUsed) ? App.Instance.Protector.Protect(_privateKey) : "",
                Passphrase = (privKeyUsed && !string.IsNullOrEmpty(passphrase)) ? App.Instance.Protector.Protect(passphrase) : ""
            };
            
            if (manageMode == 0)
            {
                Console.WriteLine(NameTextBox.Text);
                config.Connections[index] = newConnection;
                string json =  JsonSerializer.Serialize(config);
                File.WriteAllText(configPath, json);
                var box = MessageBoxManager.GetMessageBoxStandard("Connection changed successfully",
                    "Connection changed.", ButtonEnum.Ok);
                MainWindow.Instance.ConnectionsList.Children
                    .OfType<Button>()
                    .ElementAt(index).Content = NameTextBox.Text;
                MainWindow.Instance.ConnectionsList.Children
                    .OfType<Button>()
                    .ElementAt(index).Tag = newConnection;
                var grid = (Grid)ParentPanel.Children[index];
                var textblock = (TextBlock)grid.Children[0];
                textblock.Text = NameTextBox.Text;
                ResetInputFields();
                BoxPanel.IsVisible = false;
                await box.ShowAsync();
            }else if (manageMode == 1)
            {
                config.Connections.Add(newConnection);
                string json =  JsonSerializer.Serialize(config);
                File.WriteAllText(configPath, json);
                var box = MessageBoxManager.GetMessageBoxStandard("Connection added successfully",
                    "Connection added.", ButtonEnum.Ok);
                AddConnection(newConnection, ParentPanel.Children.Count());
                MainWindow.Instance.AddConnection(newConnection);
                ResetInputFields();
                BoxPanel.IsVisible = false;
                await box.ShowAsync();
            }
        }
        else
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Invalid parameters",
                "Please enter valid connection parameters", ButtonEnum.Ok);
            await box.ShowAsync();
        }
        
    }

    private void ResetInputFields()
    {
        NameTextBox.Text = "";
        HostTextBox.Text = "";
        PortTextBox.Text = "";
        UserTextBox.Text = "";
        PasswordTextBox.Text = "";
        PassphraseTextBox.Text = "";
    }

    private void ShowPasswordButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var button = sender as Button;

        if (button.Tag == "PasswordAuth")
        {
            if (PasswordTextBox.RevealPassword == false)
            {
                PasswordTextBox.RevealPassword = true;
                button.Content = "Hide Password";
            }
            else
            {
                PasswordTextBox.RevealPassword = false;
                button.Content = "Show Password";
            }
        }
        else if (button.Tag == "PrivAuth")
        {
            if (PassphraseTextBox.RevealPassword == false)
            {
                PassphraseTextBox.RevealPassword = true;
                button.Content = "Hide Password";
            }
            else
            {
                PassphraseTextBox.RevealPassword = false;
                button.Content = "Show Password";
            }
        }
        
    }

    private void AuthSelection_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (BoxPanel is null) return;
        
        if (BoxPanel.IsVisible)
        {
            if (AuthSelection.SelectedIndex == 0)
            {
                PrivateKeyAuth.IsVisible = false;
                PasswordAuth.IsVisible = true;
            } else if (AuthSelection.SelectedIndex == 1)
            {
                PasswordAuth.IsVisible = false;
                PrivateKeyAuth.IsVisible = true;
            }
        }
    }
    
    private string _privateKey = string.Empty;

    private async void PickPrivateKey_OnClick(object? sender, RoutedEventArgs e)
    {
        var files = await TopLevel.GetTopLevel(this)!
            .StorageProvider
            .OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select private key file",
                AllowMultiple = false
            });

        if (files.Count > 0)
        {
            PrivateKeyPathBox.Text = files[0].Path.LocalPath;
            // Key einlesen:
            _privateKey = await File.ReadAllTextAsync(files[0].Path.LocalPath);
        }
    }
}