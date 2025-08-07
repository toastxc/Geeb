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
using System.Media;
using LibVLCSharp.Shared;

namespace MyApp;


class CMediaPlayer
{
    public api.PlayQueue.Root? PlayQueue { get; set; } = null;
    public int PlayPosition = 0;
    public bool IsOpen { get; set; } = false;
    public MediaPlayer? Vlc { get; set; } = null;

}


public partial class MainWindow : Window




{
    private api.Login.Root? _user = null; // Modifiable
    private api.Volumes.Root? _volumes = null;
    private int _page = 0;
    private api.Albums.Root? _albums = null;
    private int? _player = null;
    private CMediaPlayer _mediaPlayer = new CMediaPlayer();
    private string? _server = null;



    public MainWindow()
    {


        InitializeComponent();

        OnStart();
        PageSelect();




    }
    private async void LoginClick(object sender, RoutedEventArgs e)
    {

        _user = await api.Login.Req(FormServer.Text, FormUsername.Text, FormPassword.Text);
        await File.WriteAllTextAsync("user.json", JsonSerializer.Serialize(_user));
        _server = FormServer.Text;
        Console.WriteLine(_server);
        await File.WriteAllTextAsync(".server", _server);


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
            _server = File.ReadAllText(".server");

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




        _albums = await api.Albums.Req(_user, FormServer.Text, _volumes.Items[VolumeList.SelectedIndex].Id);

        await File.WriteAllTextAsync("albums.json", JsonSerializer.Serialize(_albums));
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
        StackPanelMediaPlayer.IsVisible = _player != null;

        switch (Window.Width > 600, _player != null)
        {

            // big screen and player
            case (true, true):
                AlbumList.IsVisible = true;
                StackPanelMediaPlayer.Width = Window.Width / 2;
                AlbumList.Width = Window.Width / 2;
                break;
            // small screen and player
            case (false, true):
                AlbumList.IsVisible = false;
                StackPanelMediaPlayer.Width = Window.Width - 100;
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

    private void AlbumList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {

        UpdateMediaPlayer();
    }

    async private void UpdateMediaPlayer()
    {

        var album = _albums.Items[AlbumList.SelectedIndex];


        _mediaPlayer.PlayQueue = await api.PlayQueue.Req(_user, _server, album.Id);
        MpTitle.Text = album.AlbumArtist;
        MpArtists.Text = string.Join(", ", album.Artists.ToArray());
        MpQueue.Children.Clear();
        foreach (var item in _mediaPlayer.PlayQueue.Items)
        {
            MpQueue.Children.Add(new TextBlock() { Text = item.Name });
        }
    }

    async private Task music_init()
    {

        MpButtons.IsEnabled = false;
        MpPause.Content = "⌛";

        if (!Directory.Exists("songs"))
        {
            Directory.CreateDirectory("songs");
        }
        var id = _mediaPlayer.PlayQueue.Items[_mediaPlayer.PlayPosition].Id;

        var song_path = $"songs/{id}.flac";

        if (!File.Exists(song_path))
        {
            File.WriteAllBytes($"songs/{id}.flac", await api.Flac.Req(_user, id, _server));
        }


        _mediaPlayer.Vlc = new MediaPlayer(new Media(new LibVLC(enableDebugLogs: false), new Uri($"/home/kaiaxc/projects/MyApp/bin/Debug/net9.0/songs/{id}.flac")));
        MpPause.Content = "⏸";
        MpButtons.IsEnabled = true;
        await Task.Run(() =>
        {
            _mediaPlayer.Vlc.Play();
        
            while (_mediaPlayer.Vlc.WillPlay)
            {
            }

        });
      
    }

    private async void MpStopStart_OnClick(object? sender, RoutedEventArgs e)
    {
    
        
        if (_mediaPlayer.Vlc == null)
        {
            await music_init();
           
            stop();
            

        }
        else if (_mediaPlayer.Vlc.CanPause)
        {

            if (_mediaPlayer.Vlc.IsPlaying)
            {
                MpPause.Content = "⏵";
            }
            else
            {
                MpPause.Content = "⏸";
            }
            _mediaPlayer.Vlc.Pause();
        }
        else
        {
            Console.WriteLine("MP: exception");
        }
    
    }

    private void MpStop_OnClick(object? sender, RoutedEventArgs e)
    {
        stop();

    }

    private void stop()
    {
        if (_mediaPlayer.Vlc != null)
        {
            _mediaPlayer.Vlc.Stop();
            _mediaPlayer.Vlc = null;
            MpPause.Content = "⏵";
        }
    }
    private void MpSkip_OnClick(object? sender, RoutedEventArgs e)
    {


      
        SkipForward();




    }

    private void SkipForward()
    {
        if (_mediaPlayer.PlayPosition + 1 < _mediaPlayer.PlayQueue.Items.Count)
        {
            stop();
            _mediaPlayer.PlayPosition += 1;
            music_init();
        }
    }

    private void MpSkipBack_OnClick(object? sender, RoutedEventArgs e)
    {

        if (_mediaPlayer.PlayPosition > 0)
        {
            stop();
            _mediaPlayer.PlayPosition -= 1;
            music_init();
        }



    }
}



