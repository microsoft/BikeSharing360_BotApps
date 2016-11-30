#load "GeoLocation.csx"
#load "Consts.csx"

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

public class BikeData
{
    public string bikeid;
    public string serialnumber;

    public static Task<List<BikeData>> LookupBikesWithUser(string userid)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(Consts._BikeSharing360CustomerServiceApiBaseAddress);
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        string parameter = string.Format(Consts.LookupBikesWithUserAPI, userid);
        HttpResponseMessage response = client.GetAsync(parameter).Result;
        if (response.IsSuccessStatusCode)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            String responseString = await response.Content.ReadAsStringAsync();
            var responseElement = JsonConvert.DeserializeObject<List<BikeData>>(responseString, settings);
            return responseElement;
        }
        else
        {
            return null;
        }
    }

    public static Task<GeoLocation> LocateBike(string bikeid, DateTime time)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(Consts._BikeSharing360CustomerServiceApiBaseAddress);
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        string parameter = string.Format(Consts.LocateBikebyTimeAPI, bikeid, time.ToString());
        HttpResponseMessage response = client.GetAsync(parameter).Result;
        if (response.IsSuccessStatusCode)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            String responseString = await response.Content.ReadAsStringAsync();
            var responseElement = JsonConvert.DeserializeObject<GeoLocation>(responseString, settings);
            return responseElement;
        }
        else
        {            
            return null;
        }
    }

    public static Task<GeoLocation> LocateBike(string bikeid)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(Consts._BikeSharing360CustomerServiceApiBaseAddress);
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        string parameter = string.Format(Consts.LocateBikeAPI, bikeid);
        HttpResponseMessage response = client.GetAsync(parameter).Result;
        if (response.IsSuccessStatusCode)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            String responseString = await response.Content.ReadAsStringAsync();
            var responseElement = JsonConvert.DeserializeObject<GeoLocation>(responseString, settings);
            return responseElement;
        }
        else
        {
            return null;
        }
    }
}
