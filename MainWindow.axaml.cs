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
    private api.Login.Root? _user = null; // Modifiable
    private api.Volumes.Root? _volumes = null;
    private int _page = 0;
    private api.Albums.Root? _albums = null;
    private int? _player = null;
    private api.Album.Root? _album = null;


    public MainWindow()
    {


        InitializeComponent();

        OnStart();
        Console.WriteLine($"start up finished, page: {_page}");
        PageSelect();




    }
    private async void LoginClick(object sender, RoutedEventArgs e)
    {


        _user = await api.Login.Req(FormServer.Text, FormUsername.Text, FormPassword.Text);
        await File.WriteAllTextAsync("user.json", JsonSerializer.Serialize(_user));



        _volumes = await api.Volumes.Req(_user, FormServer.Text);
        _volumes.Items = _volumes.Items.Where(i => i.CollectionType == "music").ToList();
        await File.WriteAllTextAsync("volumes.json", JsonSerializer.Serialize(_volumes));
        VolumeList.ItemsSource = _volumes.Items.Select(item => item.Name).ToList();

        _page = 1;
        PageSelect();
    }

    private async void OnStart()
    {

        if (!File.Exists("user.json"))
        {
            _page = 0;

        }
        else
        {
            _page = 1;
            _user = JsonSerializer.Deserialize<api.Login.Root>(File.ReadAllText("user.json"));
        }

        switch (File.Exists("user.json"), File.Exists("volumes.json"))
        {
            case (_, true):
                _volumes = JsonSerializer.Deserialize<api.Volumes.Root>(File.ReadAllText("volumes.json"));
                break;
            case (true, false):
                _volumes = await api.Volumes.Req(_user, FormServer.Text);
                _volumes.Items = _volumes.Items.Where(i => i.CollectionType == "music").ToList();
                await File.WriteAllTextAsync("volumes.json", JsonSerializer.Serialize(_volumes));
                break;
            case (false, false):
                break;


        }

        if (_page == 1)
        {
            var names = _volumes.Items.Select(item => item.Name).ToList();
            VolumeList.ItemsSource = names;
        }



    }




    private void PageSelect()
    {
        PageLogin0.IsVisible = _page == 0;
        PageVolumes1.IsVisible = _page == 1;
        PageAlbums2.IsVisible = _page == 2;
    }

    async private void VolumeSubmit_OnClick(object? sender, RoutedEventArgs e)
    {

        VolumeSpinner.IsActive = true;
        Console.WriteLine(VolumeList.SelectedIndex);


        _albums = await api.Albums.Req(_user, FormServer.Text, _volumes.Items[VolumeList.SelectedIndex].Id);

        AlbumList.ItemsSource = _albums.Items.Select(item => item.Name);

        _page = 2;
        PageSelect();
        VolumeSpinner.IsVisible = false;

    }


    private void PageBack(object? sender, RoutedEventArgs e)
    {
        _page -= 1;
        PageSelect();
    }

    private void WindowBase_OnResized(object? sender, WindowResizedEventArgs e)
    {
        PlayerResize();
    }


    private void PlayerResize()
    {

        Console.WriteLine(Window.Width);
        MediaPlayer.IsVisible = _player != null;

        switch (Window.Width > 600, _player != null)
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
    }




    private void MediaPlayer_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_player != null)
        {
            _player = null;
        }
        else
        {
            _player = 1;
        }
        PlayerResize();
    }

}