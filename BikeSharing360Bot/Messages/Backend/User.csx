#load "Consts.csx"

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;

public enum ConnectorType
{
    SMS,
    Skype,
    Email
};

public class User
{
    public string id;
    public string userId;
    public string gender;
    public string birthDate;
    public string firstName;
    public string lastName;
    public string skype;

    public static Task<User> LookupUser(string id, ConnectorType type)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(Consts._BikeSharing360CustomerServiceApiBaseAddress);
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        string ctype = "Skype";
        switch (type)
        {
            case ConnectorType.SMS:
                ctype = "SMS";
                break;
            case ConnectorType.Email:
                ctype = "Email";
                break;
            case ConnectorType.Skype:
                ctype = "Skype";
                break;
        }
        string parameter = string.Format(Consts.LookupUserAPI, ctype, id);
        HttpResponseMessage response = client.GetAsync(parameter).Result;
        if (response.IsSuccessStatusCode)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            String responseString = await response.Content.ReadAsStringAsync();
            var responseElement = JsonConvert.DeserializeObject<User>(responseString, settings);
            return responseElement;
        }
        else
        {            
            return null;
        }
    }
}
