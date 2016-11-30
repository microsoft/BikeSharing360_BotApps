using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class Consts
{
    public const string _BikeSharing360CustomerServiceApiBaseAddress = "_YourBackendServiceBaseUrl_";

    public const string LookupUserAPI = "lookupuser?ctype={0}&value={1}";
    public const string LookupBikesWithUserAPI = "lookupbikeswithuser?userid={0}";
    public const string LocateBikeAPI = "locatebike?bikeid={0}";
    public const string LocateBikebyTimeAPI = "locatebike?bikeid={0}&datetime={1}";
    public const string FileCaseAPI = "filecase?userid={0}&incidenttype={1}&lat={2}&lon={3}";
    public const string GetMapWithRouteAPI = "getmapwithroute?latitude1={0}&longitude1={1}&name1={2}&latitude2={3}&longitude2={4}&name2={5}";
    public const string GetETAAPI = "geteta?caseNumber={0}";
}
