using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Jellyfin.Sdk.Generated.Models;
using Microsoft.Kiota.Abstractions;
using MyApp.api;

namespace MyApp;



public partial class MainWindow : Window


{
    private static api.Login.Root? User = null; // Modifiable
    private static api.Volumes.Root? Volumes = null;
    private static int Page = 0;
    private static api.Albums.Root Albums = null;

    public MainWindow()
    {


        InitializeComponent();
     
        OnStart();
        Console.WriteLine($"start up finished, page: {Page}");
        PageSelect();




        // VolumeList.ItemsSource = new string[]
        //         {"cat", "camel", "cow", "chameleon", "mouse", "lion", "zebra" }
        //     .OrderBy(x => x);

    }
    private async void LoginClick(object sender, RoutedEventArgs e)
    {

        User = await api.Login.Req(FormServer.Text, FormUsername.Text, FormPassword.Text);
        await File.WriteAllTextAsync("user.json", JsonSerializer.Serialize(User));

        Volumes = await api.Volumes.Req(User, FormServer.Text);
        Volumes.Items = Volumes.Items.Where(i => i.CollectionType == "music").ToList();
        await File.WriteAllTextAsync("volumes.json", JsonSerializer.Serialize(Volumes));


    }

    private async void OnStart()
    {
        
        if (!File.Exists("user.json"))
        {
            Page = 0;

        }
        else
        {
            Page = 1;
            User = JsonSerializer.Deserialize<api.Login.Root>(File.ReadAllText("user.json"));
        }

        switch (File.Exists("user.json"), File.Exists("volumes.json"))
        {
            case (_, true):
                Volumes = JsonSerializer.Deserialize<api.Volumes.Root>(File.ReadAllText("volumes.json"));
                break;
            case (true, false):
                Volumes = await api.Volumes.Req(User, FormServer.Text);
                Volumes.Items = Volumes.Items.Where(i => i.CollectionType == "music").ToList();
                await File.WriteAllTextAsync("volumes.json", JsonSerializer.Serialize(Volumes));
                break;
            case (false, false):
                break;


        }

        var names = Volumes.Items.Select(item => item.Name).ToList();

        VolumeList.ItemsSource = names;



    }




    private void PageSelect()
    {
        PageLogin0.IsVisible = Page == 0;
        PageVolumes1.IsVisible = Page == 1;
        PageAlbums2.IsVisible = Page == 2;
    }

    async private void VolumeSubmit_OnClick(object? sender, RoutedEventArgs e)
    {
      
        Console.WriteLine(VolumeList.SelectedIndex);

        VolumeSpinner.IsVisible = true;
        Albums = await api.Albums.Req(User, FormServer.Text);
        // foreach (var album in Albums.Items)
        // {
            // AlbumList.Items.Append(album.Name);
        // }

        
        AlbumList.ItemsSource = Albums.Items.Select(item => item.Name );
        
      
        
        Page = 2;
        PageSelect();
        
    }


    private void AlbumBack_OnClick(object? sender, RoutedEventArgs e)
    {
        Page = 1;
        PageSelect();
    }
}
