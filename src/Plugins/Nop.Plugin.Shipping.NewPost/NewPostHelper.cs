using Nop.Core.Plugins;
using Nop.Services.Localization;
using Nop.Services.Shipping;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;
using Nop.Services.Configuration;

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
                Url = "http://testapi.novaposhta.ua/v2.0/en/",
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
            var settings = _settingService.LoadSetting<NewPostSettings>();

            return string.Empty;
        }

        public string GetCost(string cityIdFrom, string cityIdTo)
        {
            var settings = _settingService.LoadSetting<NewPostSettings>();

            return string.Empty;
        }

        #endregion
    }
}
