#load "Consts.csx"
#load "GeoLocation.csx"

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

public class BingMapHelper
{

    public static async Task<string> HighlightRoute(GeoLocation location1, GeoLocation location2)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(Consts._BikeSharing360CustomerServiceApiBaseAddress);
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        string parameter = string.Format(Consts.GetMapWithRouteAPI,
            location1.latitude, location1.longitude, location1.name,
            location2.latitude, location2.longitude, location2.name);
        HttpResponseMessage response = client.GetAsync(parameter).Result;
        if (response.IsSuccessStatusCode)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            var responseString = await response.Content.ReadAsStringAsync();
            var responseElement = JsonConvert.DeserializeObject<string>(responseString, settings);
            return responseElement;
        }
        else
        {
            return "";
        }
    }
}
