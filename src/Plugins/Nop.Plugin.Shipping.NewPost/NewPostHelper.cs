using Nop.Core.Plugins;
using Nop.Services.Localization;
using Nop.Services.Shipping;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Plugin.Shipping.NewPost.Domain;
using System.Net.Http;
using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.NewPost
{
    public class NewPostHelper : BasePlugin, INewPostHelper
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public NewPostHelper(ILocalizationService localizationService,
            ILogger logger,
            IWebHelper webHelper,
            ISettingService settingService)
        {
            this._localizationService = localizationService;
            this._logger = logger;
            this._webHelper = webHelper;
            this._settingService = settingService;
        }

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/NewPost/Configure";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new NewPostSettings
            {
                Url = "http://testapi.novaposhta.ua/v2.0/",
                ApiKey = "45f32a18b7f954ee48e88d17544ceaaa"
            };
            _settingService.SaveSetting(settings);

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.NewPost.Fields.ApiKey", "Ключ к API новой почты");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.NewPost.Fields.Url", "Базовый URL к API новой почты");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            _settingService.DeleteSetting<NewPostSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.NewPost.Fields.ApiKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.NewPost.Fields.Url");

            base.Uninstall();
        }

        public string GetCityId(string cityName)
        {
            var cityId = string.Empty;
            var settings = _settingService.LoadSetting<NewPostSettings>();
            var request = new NewPostApiRequest<NewPostAddressSearchSettlements>()
            {
                apiKey = settings.ApiKey,
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
                client.BaseAddress = new Uri(settings.Url);
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
                _logger.Error("New post has been throw exception when does the GetCityId", ex);
            }

            return cityId;
        }

        public int GetCost(string cityIdFrom, string cityIdTo, string weight, string assessedCost)
        {
            int cost = -1;
            var settings = _settingService.LoadSetting<NewPostSettings>();
            var request = new NewPostApiRequest<NewPostInternetDocumentGetDocumentPrice>()
            {
                apiKey = settings.ApiKey,
                modelName = "Address",
                calledMethod = "searchSettlements",
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
                client.BaseAddress = new Uri(settings.Url);
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
                _logger.Error("New post has been throw exception when does the GetCost", ex);
            }

            return cost;
        }

        #endregion
    }
}
