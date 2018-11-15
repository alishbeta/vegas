using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Security
{
    public class FreeGeoIpModel
    {
        public string ip { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
        public string region_code { get; set; }
        public string region_name { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        //public int metro_code { get; set; }
        //{
        //  "ip": "5.9.13.82",
        //  "country_code": "DE",
        //  "country_name": "Germany",
        //  "region_code": "",
        //  "region_name": "",
        //  "city": "",
        //  "zip_code": "",
        //  "time_zone": "",
        //  "latitude": 51.2993,
        //  "longitude": 9.491,
        //  "metro_code": 0
        //}
    }
}
