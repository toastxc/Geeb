using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;

namespace MyApp.api;

public class Album
{
    public static async Task<Albums.Root> Req(api.Login.Root user, string url, string album)
    {

        var client = new RestSharp.RestClient($"https://{url}/Users/{user.User.Id}/Items/{album}");
        var request = new RestRequest("", Method.Get);
        request.AddHeader("authorization", $"MediaBrowser Client=\"Geeb\", Device=\"Linux\", DeviceId=\"23458723472389\", Version=\"10.10.3\", Token=\"{user.AccessToken}\"");
        return JsonSerializer.Deserialize<Albums.Root>(client.Execute(request).Content);

    }
    public class AlbumArtist
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class ArtistItem
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class GenreItem
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class ImageBlurHashes
    {
        public Primary Primary { get; set; }
    }

    public class ImageTags
    {
        public string Primary { get; set; }
    }

    public class Primary
    {
        public string a6e294776f1190febfe8b9522b3d2b92 { get; set; }
    }

    public class ProviderIds
    {
    }

    public class Root
    {
        public string Name { get; set; }
        public string ServerId { get; set; }
        public string Id { get; set; }
        public string Etag { get; set; }
        public DateTime DateCreated { get; set; }
        public bool CanDelete { get; set; }
        public bool CanDownload { get; set; }
        public string SortName { get; set; }
        public DateTime PremiereDate { get; set; }
        public List<object> ExternalUrls { get; set; }
        public string Path { get; set; }
        public bool EnableMediaSourceDisplay { get; set; }
        public object ChannelId { get; set; }
        public List<object> Taglines { get; set; }
        public List<string> Genres { get; set; }
        public long CumulativeRunTimeTicks { get; set; }
        public long RunTimeTicks { get; set; }
        public string PlayAccess { get; set; }
        public int ProductionYear { get; set; }
        public List<object> RemoteTrailers { get; set; }
        public ProviderIds ProviderIds { get; set; }
        public bool IsFolder { get; set; }
        public string ParentId { get; set; }
        public string Type { get; set; }
        public List<object> People { get; set; }
        public List<object> Studios { get; set; }
        public List<GenreItem> GenreItems { get; set; }
        public int LocalTrailerCount { get; set; }
        public UserData UserData { get; set; }
        public int RecursiveItemCount { get; set; }
        public int ChildCount { get; set; }
        public int SpecialFeatureCount { get; set; }
        public string DisplayPreferencesId { get; set; }
        public List<object> Tags { get; set; }
        public int PrimaryImageAspectRatio { get; set; }
        public List<string> Artists { get; set; }
        public List<ArtistItem> ArtistItems { get; set; }
        public string AlbumArtist { get; set; }
        public List<AlbumArtist> AlbumArtists { get; set; }
        public ImageTags ImageTags { get; set; }
        public List<object> BackdropImageTags { get; set; }
        public ImageBlurHashes ImageBlurHashes { get; set; }
        public string LocationType { get; set; }
        public string MediaType { get; set; }
        public List<object> LockedFields { get; set; }
        public bool LockData { get; set; }
    }

    public class UserData
    {
        public int PlaybackPositionTicks { get; set; }
        public int PlayCount { get; set; }
        public bool IsFavorite { get; set; }
        public bool Played { get; set; }
        public string Key { get; set; }
        public string ItemId { get; set; }
    }


}


