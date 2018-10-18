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
            return $"{_webHelper.GetStoreLocation()}Admin/ShippingNewPost/Configure";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new NewPostSettings
            {
                Url = "https://www.ups.com/ups.app/xml/Rate",
                ApiKey = string.Empty
            };
            _settingService.SaveSetting(settings);

            //locales
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExchangeRate.EcbExchange.Error", "You can use ECB (European central bank) exchange rate provider only when the primary exchange rate currency is supported by ECB");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            _settingService.DeleteSetting<NewPostSettings>();

            //locales
            //_localizationService.DeletePluginLocaleResource("Plugins.ExchangeRate.EcbExchange.Error");

            base.Uninstall();
        }

        #endregion
    }
}
