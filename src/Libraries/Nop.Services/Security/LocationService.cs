using Newtonsoft.Json;
using Nop.Core.Domain.Security;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Security
{
    public class LocationService : ILocationService
    {
        //HttpContext.Current.Request.UserHostAddress
        virtual public FreeGeoIpModel GetLocation(string ip)
        {
            FreeGeoIpModel model = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://api.ipstack.com");
                Task<HttpResponseMessage> task = client.GetAsync(String.Format("/{0}?access_key=be356d5ceea1f080fcc6924a02328824", ip));//http://freegeoip.net/json/{ip}
                task.Wait();
                HttpResponseMessage response = task.Result;
                if (response.IsSuccessStatusCode)
                {
                    Task<Stream> taskStream = response.Content.ReadAsStreamAsync();
                    taskStream.Wait();
                    using (var reader = new StreamReader((taskStream.Result), Encoding.UTF8))
                    {
                        model = JsonConvert.DeserializeObject<FreeGeoIpModel>(reader.ReadToEnd());
                    }
                }
            }
            return model;
        }
    }
}
