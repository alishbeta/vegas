using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.LiqPay.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.LiqPay.Controllers
{
    public class PaymentLiqPayController : BasePaymentController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPermissionService _permissionService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public PaymentLiqPayController(IWorkContext workContext,
            ISettingService settingService,
            IPaymentService paymentService,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            IPermissionService permissionService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            ILogger logger,
            IWebHelper webHelper,
            ShoppingCartSettings shoppingCartSettings)
        {
            this._workContext = workContext;
            this._settingService = settingService;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._permissionService = permissionService;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._storeContext = storeContext;
            this._logger = logger;
            this._webHelper = webHelper;
            this._shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
			if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
				return AccessDeniedView();

			//load settings for a chosen store scope
			var storeScope = _storeContext.ActiveStoreScopeConfiguration;
			var liqPayPaymentSettings = _settingService.LoadSetting<LiqPayPaymentSettings>(storeScope);

			var model = new ConfigurationModel
			{
				UseSandbox = liqPayPaymentSettings.UseSandbox,
				PublicKey = liqPayPaymentSettings.PublicKey,
				PrivateKey = liqPayPaymentSettings.PrivateKey,
				ActiveStoreScopeConfiguration = storeScope
			};
			if (storeScope > 0)
			{
				model.UseSandbox_OverrideForStore = _settingService.SettingExists(liqPayPaymentSettings, x => x.UseSandbox, storeScope);
				model.PublicKey_OverrideForStore = _settingService.SettingExists(liqPayPaymentSettings, x => x.PublicKey, storeScope);
				model.PrivateKey_OverrideForStore = _settingService.SettingExists(liqPayPaymentSettings, x => x.PrivateKey, storeScope);
			}

			return View("~/Plugins/Payments.LiqPay/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
			if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
				return AccessDeniedView();

			if (!ModelState.IsValid)
				return Configure();

			//load settings for a chosen store scope
			var storeScope = _storeContext.ActiveStoreScopeConfiguration;
			var liqPayPaymentSettings = _settingService.LoadSetting<LiqPayPaymentSettings>(storeScope);

			//save settings
			liqPayPaymentSettings.UseSandbox = model.UseSandbox;
			liqPayPaymentSettings.PublicKey = model.PublicKey;
			liqPayPaymentSettings.PrivateKey = model.PrivateKey;

			/* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
			_settingService.SaveSettingOverridablePerStore(liqPayPaymentSettings, x => x.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
			_settingService.SaveSettingOverridablePerStore(liqPayPaymentSettings, x => x.PublicKey, model.PublicKey_OverrideForStore, storeScope, false);
			_settingService.SaveSettingOverridablePerStore(liqPayPaymentSettings, x => x.PrivateKey, model.PrivateKey_OverrideForStore, storeScope, false);

			//now clear settings cache
			_settingService.ClearCache();

			SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

			return Configure();
        }

        //action displaying notification (warning) to a store owner about inaccurate PayPal rounding
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult RoundingWarning(bool passProductNamesAndTotals)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //prices and total aren't rounded, so display warning
            if (passProductNamesAndTotals && !_shoppingCartSettings.RoundPricesDuringCalculation)
                return Json(new { Result = _localizationService.GetResource("Plugins.Payments.LiqPay.RoundingWarning") });

            return Json(new { Result = string.Empty });
        }

        public IActionResult CancelOrder()
        {
			var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
				customerId: _workContext.CurrentCustomer.Id, pageSize: 1).FirstOrDefault();
			if (order != null)
				return RedirectToRoute("OrderDetails", new { orderId = order.Id });

			return RedirectToRoute("HomePage");
        }

        #endregion
    }
}