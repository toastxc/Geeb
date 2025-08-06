using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;

namespace MyApp.api;

public class PlayQueue
{
    public static async Task<Root> Req(api.Login.Root user, string url, string album)
    {
        var client = new RestSharp.RestClient($"{url}/Users/{user.User.Id}/Items?ParentId={album}&Filters=IsNotFolder&Recursive=true&SortBy=SortName&MediaTypes=Audio%252CVideo&SortOrder=Ascending&Limit=300&Fields=Chapters%252CTrickplay&ExcludeLocationTypes=Virtual&EnableTotalRecordCount=false&CollapseBoxSetItems=false");
        var request = new RestRequest("", Method.Get);
        request.AddHeader("authorization", $"MediaBrowser Client=\"Geeb\", Device=\"Linux\", DeviceId=\"23458723472389\", Version=\"10.10.3\", Token=\"{user.AccessToken}\"");
        return JsonSerializer.Deserialize<Root>(client.Execute(request).Content);

    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
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



    public class ImageTags
    {
        public string Primary { get; set; }
    }

    public class Item
    {
        public string Name { get; set; }
        public string ServerId { get; set; }
        public string Id { get; set; }
        public bool HasLyrics { get; set; }
        public string Container { get; set; }
        public DateTime PremiereDate { get; set; }
        public object ChannelId { get; set; }
        public double RunTimeTicks { get; set; }
        public int ProductionYear { get; set; }
        public int IndexNumber { get; set; }
        public int ParentIndexNumber { get; set; }
        public bool IsFolder { get; set; }
        public string Type { get; set; }
        public UserData UserData { get; set; }
        public List<string> Artists { get; set; }
        public List<ArtistItem> ArtistItems { get; set; }
        public string Album { get; set; }
        public string AlbumId { get; set; }
        public string AlbumPrimaryImageTag { get; set; }
        public string AlbumArtist { get; set; }
        public List<AlbumArtist> AlbumArtists { get; set; }
        public ImageTags ImageTags { get; set; }
        public List<object> BackdropImageTags { get; set; }
        public string LocationType { get; set; }
        public string MediaType { get; set; }
    }


    public class Root
    {
        public List<Item> Items { get; set; }
        public int TotalRecordCount { get; set; }
        public int StartIndex { get; set; }
    }

    public class UserData
    {
        public int PlaybackPositionTicks { get; set; }
        public int PlayCount { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime LastPlayedDate { get; set; }
        public bool Played { get; set; }
        public string Key { get; set; }
        public string ItemId { get; set; }
    }




}


