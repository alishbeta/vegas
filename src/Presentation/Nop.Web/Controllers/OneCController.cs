using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.OneC;
using Nop.Services.Customers;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.OneC;

namespace Nop.Web.Controllers
{
    public class OneCController : BasePublicController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IImportManager _importManager;
        private readonly IOrderService _orderService;

        #endregion

        #region Ctor

        public OneCController(IAddressModelFactory addressModelFactory,
            ILocalizationService localizationService,
            ICustomerService customerService,
            ICustomerRegistrationService customerRegistrationService,
            CustomerSettings customerSettings,
            ICustomerActivityService customerActivityService,
            IImportManager importManager,
            IOrderService orderService)
        {
            this._localizationService = localizationService;
            this._customerService = customerService;
            this._customerRegistrationService = customerRegistrationService;
            this._customerSettings = customerSettings;
            this._customerActivityService = customerActivityService;
            this._importManager = importManager;
            this._orderService = orderService;
        }

        #endregion

        [HttpPost]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual JsonResult ImportProducts([FromBody]OneCProductsImport model)
        {
            var response = new OneCResponse
            {
                Success = false
            };

            if (_customerSettings.UsernamesEnabled && model.Username != null)
            {
                model.Username = model.Username.Trim();
            }

            var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
            switch (loginResult)
            {
                case CustomerLoginResults.Successful:
                    {
                        var customer = _customerSettings.UsernamesEnabled
                            ? _customerService.GetCustomerByUsername(model.Username)
                            : _customerService.GetCustomerByEmail(model.Email);

                        //activity log
                        _customerActivityService.InsertActivity(customer, "PublicStore.1C.ImportProducts.Login", "1C Importing products begin.");

                        if (model.Products != null)
                        {
                            //import product
                            var result = _importManager.ImportProductsFromOneC(model.Products);
                            response.Success = result.Item1;
                            response.Message = result.Item2;
                        }
                        else
                        {
                            response.Message = "Products is null.";
                        }

                        //activiti log
                        _customerActivityService.InsertActivity(customer, "PublicStore.1C.ImportProducts.LogOut", "1C Importing products end.");
                        break;
                    }
                case CustomerLoginResults.CustomerNotExist:
                    response.Message = _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist");
                    break;
                case CustomerLoginResults.Deleted:
                    response.Message = _localizationService.GetResource("Account.Login.WrongCredentials.Deleted");
                    break;
                case CustomerLoginResults.NotActive:
                    response.Message = _localizationService.GetResource("Account.Login.WrongCredentials.NotActive");
                    break;
                case CustomerLoginResults.NotRegistered:
                    response.Message = _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered");
                    break;
                case CustomerLoginResults.LockedOut:
                    response.Message = _localizationService.GetResource("Account.Login.WrongCredentials.LockedOut");
                    break;
                case CustomerLoginResults.WrongPassword:
                default:
                    response.Message = _localizationService.GetResource("Account.Login.WrongCredentials");
                    break;
            }
            return Json(response);
        }

        [HttpPost]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual JsonResult ExportOrders([FromBody]OneCProductsImport model)
        {
            //_orderService
            return Json(null);
        }

        [HttpPost]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual JsonResult Test()
        {
            return Json(new OneCProductsImport() { Products = new System.Collections.Generic.List<OneCProduct>() { new OneCProduct() { Attributes = new System.Collections.Generic.List<OneCAttribute>() { new OneCAttribute() { AttributeValues = new System.Collections.Generic.List<OneCAttributeValue>() { new OneCAttributeValue() } } } } } });
        }
    }
}
