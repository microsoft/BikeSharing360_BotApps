#load "Consts.csx"

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Text;

public enum IncidentType
{
    lost,
    flattire
};

public class CustomerService
{
    public static async Task<string> FileCase(string userid, IncidentType incidenttype, double lat, double lon)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(Consts._BikeSharing360CustomerServiceApiBaseAddress);
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        string ctype = "lost";
        switch (incidenttype)
        {
            case IncidentType.flattire:
                ctype = "flattire";
                break;
            case IncidentType.lost:
                ctype = "lost";
                break;
        }
        string parameter = string.Format(Consts.FileCaseAPI, userid, ctype, lat, lon);
        HttpResponseMessage response = client.GetAsync(parameter).Result;
        if (response.IsSuccessStatusCode)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            String responseString = await response.Content.ReadAsStringAsync();
            var responseElement = JsonConvert.DeserializeObject<string>(responseString, settings);
            return responseElement;
        }
        else
        {
            return "";
        }
    }
    public static async Task<int> GetETA(string caseNumber)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(Consts._BikeSharing360CustomerServiceApiBaseAddress);
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        string parameter = string.Format(Consts.GetETAAPI, caseNumber);
        HttpResponseMessage response = client.GetAsync(parameter).Result;
        if (response.IsSuccessStatusCode)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            var responseString = await response.Content.ReadAsStringAsync();
            var responseElement = JsonConvert.DeserializeObject<int>(responseString, settings);
            return responseElement;
        }
        else
        {
            return -1;
        }
    }
}
