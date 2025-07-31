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
        request.AddHeader("authorization", $"MediaBrowser Client=\"Geeb\", Device=\"Linux\", DeviceId=\"23458723472389\", Version=\"10.10.3\", Token=\"{user.AccessToken}\"");

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