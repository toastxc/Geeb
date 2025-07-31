using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
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
    private static int? Player = null;

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
        VolumeList.ItemsSource = Volumes.Items.Select(item => item.Name).ToList();

        Page = 1;
        PageSelect();
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

        if (Page == 1)
        {
            var names = Volumes.Items.Select(item => item.Name).ToList();
            VolumeList.ItemsSource = names;
        }



    }




    private void PageSelect()
    {
        PageLogin0.IsVisible = Page == 0;
        PageVolumes1.IsVisible = Page == 1;
        PageAlbums2.IsVisible = Page == 2;
    }

    async private void VolumeSubmit_OnClick(object? sender, RoutedEventArgs e)
    {

        VolumeSpinner.IsActive = true;
        Console.WriteLine(VolumeList.SelectedIndex);


        Albums = await api.Albums.Req(User, FormServer.Text, Volumes.Items[VolumeList.SelectedIndex].Id);

        AlbumList.ItemsSource = Albums.Items.Select(item => item.Name);

        Page = 2;
        PageSelect();
        VolumeSpinner.IsVisible = false;

    }


    private void PageBack(object? sender, RoutedEventArgs e)
    {
        Page -= 1;
        PageSelect();
    }

    private void WindowBase_OnResized(object? sender, WindowResizedEventArgs e)
    {
        PlayerResize();
    }


    private void PlayerResize()
    {

        Console.WriteLine(Window.Width);
        MediaPlayer.IsVisible = Player != null;
        
        switch (Window.Width > 600, Player != null)
        {
               
            // big screen and player
            case (true, true):
                AlbumList.IsVisible = true;
                MediaPlayer.Width = Window.Width / 2; 
                AlbumList.Width = Window.Width / 2; 
                break;
            // small screen and player
            case (false, true):
                AlbumList.IsVisible = false;
                MediaPlayer.Width = Window.Width - 100; 
                break;
            // small screen and no player
            case (_, false):
                AlbumList.IsVisible = true;
                AlbumList.Width = Window.Width - 100; 
                break;
          
        }
        AlbumList.Height = Window.Height - 200;
        // if (Player != null)
        // {
        //     MediaPlayer.IsVisible = true;
        //     AlbumList.Width = Window.Width / 2;
        // }
        // else
        // {
        //     MediaPlayer.IsVisible = false;
        //     AlbumList.Width = Window.Width - 100;
        }


    

    private void MediaPlayer_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Player != null)
        {
            Player = null;
        }
        else
        {
            Player = 1;
        }
        PlayerResize();
    }
}