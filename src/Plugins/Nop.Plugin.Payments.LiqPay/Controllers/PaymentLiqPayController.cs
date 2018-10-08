using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

		private string _baseUrl = "https://vegas.mo-apps.com/";
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
		private readonly LiqPayPaymentSettings _liqPayPaymentSettings;

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
            ShoppingCartSettings shoppingCartSettings,
			LiqPayPaymentSettings liqPayPaymentSettings)
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
			this._liqPayPaymentSettings = liqPayPaymentSettings;
		}

		#endregion

		#region Methods

		public IActionResult CreatePayment(int orderId)
		{
			PaymentLiqpayVM model = new PaymentLiqpayVM();
			LiqPaySignatureGenerator liqpaysignature = new LiqPaySignatureGenerator();
			var order = _orderService.GetOrderById(orderId); 
			if (order == null)
			{
				return Redirect(_baseUrl);
			}
			if (order.PaymentStatus == PaymentStatus.Paid)
			{
				return Redirect(string.Format("{0}ru/checkout/completed/{1}", _baseUrl, order.Id));
			}
			model.pay_way = "card,liqpay,privat24";
			model.amount = order.OrderTotal.ToString("0.00", CultureInfo.InvariantCulture);
			model.currency = "UAH";
			model.action = "pay";
			model.private_key = _liqPayPaymentSettings.PrivateKey;
			model.public_key = _liqPayPaymentSettings.PublicKey;
			model.recurringbytoken = "0";
			model.result_url = string.Format("{0}ru/checkout/completed/{1}", _baseUrl, order.Id); ;
			model.server_url = string.Format("{0}liqpay/callback", _baseUrl);
			model.version = "3";
			if (_liqPayPaymentSettings.UseSandbox)
			{
				model.sandbox = "1";
			}
			else
			{
				model.sandbox = "0";
			}
			model.description = String.Format("Заказ #{0}. Имя: {1}, тел. {2}, email: {3}", order.Id, order.Customer.BillingAddress.FirstName, order.Customer.BillingAddress.PhoneNumber, order.Customer.Email);

			model.order_id = order.Id.ToString();
			liqpaysignature.GenerateSignature(model);

			return View("~/Plugins/Payments.LiqPay/Views/CreatePayment.cshtml", model);
		}

		[HttpPost]
		public string ServerUrl(string signature, string data)
		{
			try
			{
				if (signature == null || data == null) throw new Exception("ERROR_EMPTY_DATA");

				byte[] dataVal = Convert.FromBase64String(data);
				string decodedString = Encoding.UTF8.GetString(dataVal);

				PaymentLiqpayResponse model = JsonConvert.DeserializeObject<PaymentLiqpayResponse>(decodedString);

				LiqPaySignatureGenerator liqpaySignature = new LiqPaySignatureGenerator();
				if (!String.Equals(signature, liqpaySignature.GenerateSignature(data, _liqPayPaymentSettings.PrivateKey)))
				{
					throw new Exception("SENDER_ERROR");
				}
				var order = _orderService.GetOrderById(int.Parse(model.order_id));
				if (order == null)
				{
					throw new Exception("ORDER_NOT_FOUND");
				}
				if (model.status == "success" || model.status == "sandbox")
				{
					order.PaymentStatus = PaymentStatus.Paid;
				}
				if (model.status == "failure")
				{
					order.PaymentStatus = PaymentStatus.Voided;
				}  
				if (model.status == "processing")
				{
					order.PaymentStatus = PaymentStatus.Pending;
				}
				_orderService.UpdateOrder(order);
				return "OK";
			}
			catch (Exception ex)
			{
				_logger.InsertLog(Core.Domain.Logging.LogLevel.Error, ex.Message, JsonConvert.SerializeObject(ex));
				throw ex;  
			}
		}


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