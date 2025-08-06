using System;
using System.Threading.Tasks;
using RestSharp;

namespace MyApp.api;
using System.IO;

public class Flac
{
   public static async Task<byte[]?> Req(api.Login.Root user, string song, string url)
    {
        var client = new RestSharp.RestClient($"{url}/Items/{song}/Download?api_key={user.AccessToken}");
        var request = new RestRequest("", Method.Get);
        return await client.DownloadDataAsync(request);





    }

}


