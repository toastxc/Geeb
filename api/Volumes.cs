using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;

namespace MyApp.api;

public class Volumes
{


    public static async Task<Root> Req(api.Login.Root user, string url)


    {
        
        
        
        var client = new RestSharp.RestClient($"{url}/Users/{user.User.Id}/Items?SortBy=SortName%255E&SortOrder=Ascending%255E&IncludeItemTypes=MusicAlbum%255E&EnableImageTypes=Primary%252CBackdrop%252CBanner%252CThumb%255E");
        var request = new RestRequest("", Method.Get);
        request.AddHeader("user-agent", "Mozilla/5.0 (X11; Linux x86_64; rv:140.0) Gecko/20100101 Firefox/140.0");
        request.AddHeader("accept", "application/json");
        request.AddHeader("accept-language", "en-US,en;q=0.5");
        request.AddHeader("accept-encoding", "gzip, deflate, br, zstd");
        request.AddHeader("authorization", $"MediaBrowser Client=\"Jellyfin Web\", Device=\"Firefox\", DeviceId=\"TW96aWxsYS81LjAgKFgxMTsgTGludXggeDg2XzY0OyBydjoxNDAuMCkgR2Vja28vMjAxMDAxMDEgRmlyZWZveC8xNDAuMHwxNzUyODMxNzc1MzAx\", Version=\"10.10.3\", Token=\"{user.AccessToken}\"");
        request.AddHeader("dnt", "1");
        request.AddHeader("sec-gpc", "1");
        request.AddHeader("connection", "keep-alive");
        request.AddHeader("sec-fetch-dest", "empty");
        request.AddHeader("sec-fetch-mode", "cors");
        request.AddHeader("sec-fetch-site", "same-origin");
        request.AddHeader("priority", "u=4");
        request.AddHeader("te", "trailers");

        return JsonSerializer.Deserialize<Root>(client.Execute(request).Content);

    }



    public class Item
    {
        public string Name { get; set; }
        public string ServerId { get; set; }
        public string Id { get; set; }
        public object ChannelId { get; set; }
        public bool IsFolder { get; set; }
        public string Type { get; set; }
        public UserData UserData { get; set; }
        public string CollectionType { get; set; }
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
        public bool Played { get; set; }
        public string Key { get; set; }
        public string ItemId { get; set; }
    }



}