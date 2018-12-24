using Castle.Core.Logging;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Nop.Core.Domain.Shipping.NewPost;

namespace Nop.Services.Shipping
{
    public class NewPostService : INewPostService
    {
        #region Fields
        private string _apiKey = "45f32a18b7f954ee48e88d17544ceaaa";
        private string _url = "http://testapi.novaposhta.ua/v2.0/";

        #endregion

        #region Methods
        
        public string GetCityId(string cityName)
        {
            var cityId = string.Empty;
            var request = new NewPostApiRequest<NewPostAddressSearchSettlements>()
            {
                apiKey = _apiKey,
                modelName = "Address",
                calledMethod = "searchSettlements",
                methodProperties = new NewPostAddressSearchSettlements()
                {
                    CityName = cityName.Trim().ToLower(),
                    Limit = 1
                }
            };

            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(_url);
                var response = client.PostAsync("json/Address/searchSettlements", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")).Result;

                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    var responce = JsonConvert.DeserializeObject<NewPostApiResponse<List<NewPostAddressList>>>(result);
                    if (responce.success && responce.data.Count > 0 && responce.data[0].Addresses.Count > 0)
                    {
                        cityId = responce.data[0].Addresses[0].DeliveryCity;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return cityId;
        }

        public int GetCost(string cityIdFrom, string cityIdTo, string weight, string assessedCost)
        {
            int cost = -1;
            var request = new NewPostApiRequest<NewPostInternetDocumentGetDocumentPrice>()
            {
                apiKey = _apiKey,
                modelName = "InternetDocument",
                calledMethod = "getDocumentPrice",
                methodProperties = new NewPostInternetDocumentGetDocumentPrice()
                {
                    CargoType = "Cargo",
                    ServiceType = "DoorsDoors",
                    CitySender = cityIdFrom,
                    CityRecipient = cityIdTo,
                    Weight = weight,
                    Cost = assessedCost
                }
            };

            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(_url);
                var response = client.PostAsync("en/getDocumentPrice/json/", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")).Result;

                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    var responce = JsonConvert.DeserializeObject<NewPostApiResponse<List<NewPostCost>>>(result);
                    if (responce.success && responce.data.Count > 0)
                    {
                        cost = responce.data[0].Cost;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return cost;
        }

        #endregion
    }
}
