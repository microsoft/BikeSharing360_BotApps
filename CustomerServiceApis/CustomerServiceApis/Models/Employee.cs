using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerServiceApis.Models
{
    public class GeoLocation
    {
        public double latitude;
        public double longitude;
        public string name;
    }
    public class BotUser
    {
        public string id;
        public string name;
        public string serviceUrl;
        public string conversationId;
        public GeoLocation location;
    }
    public class Employee
    {
        public string id;
        public string name;
        public string serviceUrl;
        public string conversationId;
        public GeoLocation location;
        public BotUser customer;
    }
}