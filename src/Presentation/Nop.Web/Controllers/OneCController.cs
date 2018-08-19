﻿using Microsoft.AspNetCore.Mvc;
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
using System.Collections.Generic;
using System.Linq;

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
        private readonly IExportManager _exportManager;

        #endregion

        #region Ctor

        public OneCController(IAddressModelFactory addressModelFactory,
            ILocalizationService localizationService,
            ICustomerService customerService,
            ICustomerRegistrationService customerRegistrationService,
            CustomerSettings customerSettings,
            ICustomerActivityService customerActivityService,
            IImportManager importManager,
            IExportManager exportManager)
        {
            this._localizationService = localizationService;
            this._customerService = customerService;
            this._customerRegistrationService = customerRegistrationService;
            this._customerSettings = customerSettings;
            this._customerActivityService = customerActivityService;
            this._importManager = importManager;
            this._exportManager = exportManager;
        }

        #endregion

        public OneCResponse IsLogin(string userName, string email, string password)
        {
            var response = new OneCResponse
            {
                Success = false
            };

            if (_customerSettings.UsernamesEnabled && userName != null)
            {
                userName = userName.Trim();
            }

            var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? userName : email, password);
            switch (loginResult)
            {
                case CustomerLoginResults.Successful:
                    response.Success = true;
                    break;
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

            return response;
        }

        [HttpPost]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual JsonResult ImportProducts([FromBody]OneCProductsImport model)
        {
            var response = IsLogin(model?.Username, model?.Email, model?.Password);
            if (response.Success)
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
                    response.Success = false;
                }

                //activiti log
                _customerActivityService.InsertActivity(customer, "PublicStore.1C.ImportProducts.LogOut", "1C Importing products end.");
            }
            return Json(response);
        }

        [HttpPost]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual JsonResult ExportOrders([FromBody]OneCAuth model)
        {
            var response = IsLogin(model?.Username, model?.Email, model?.Password);
            if (response.Success)
            {
                var customer = _customerSettings.UsernamesEnabled
                    ? _customerService.GetCustomerByUsername(model.Username)
                    : _customerService.GetCustomerByEmail(model.Email);

                //activity log
                _customerActivityService.InsertActivity(customer, "PublicStore.1C.ExportOrders.Login", "1C Exporting orders begin.");

                var orders = _exportManager.ExportOrdersToOneC();

                if (orders.Count() > 0)
                {
                    response.Data = orders;
                }
                else
                {
                    response.Success = false;
                    response.Message = "Does not have any order.";
                }

                //activiti log
                _customerActivityService.InsertActivity(customer, "PublicStore.1C.ExportOrders.LogOut", "1C Exporting orders end.");
            }
            return Json(response);
        }

        [HttpPost]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual JsonResult ExportDiscounts([FromBody]OneCAuth model)
        {
            var response = IsLogin(model?.Username, model?.Email, model?.Password);
            if (response.Success)
            {
                var customer = _customerSettings.UsernamesEnabled
                    ? _customerService.GetCustomerByUsername(model.Username)
                    : _customerService.GetCustomerByEmail(model.Email);

                //activity log
                _customerActivityService.InsertActivity(customer, "PublicStore.1C.ExportDiscounts.Login", "1C Exporting discount begin.");

                var discounts = _exportManager.ExportDiscountsToOneC();

                if (discounts.Count() > 0)
                {
                    response.Data = discounts;
                }
                else
                {
                    response.Success = false;
                    response.Message = "Does not have any discount.";
                }

                //activiti log
                _customerActivityService.InsertActivity(customer, "PublicStore.1C.ExportDiscounts.LogOut", "1C Exporting discount end.");
            }
            return Json(response);
        }

        [HttpPost]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual JsonResult ExportCustomers([FromBody]OneCAuth model)
        {
            var response = IsLogin(model?.Username, model?.Email, model?.Password);
            if (response.Success)
            {
                var customer = _customerSettings.UsernamesEnabled
                    ? _customerService.GetCustomerByUsername(model.Username)
                    : _customerService.GetCustomerByEmail(model.Email);

                //activity log
                _customerActivityService.InsertActivity(customer, "PublicStore.1C.ExportCustomers.Login", "1C Exporting customers begin.");

                var customers = _exportManager.ExportUsersToOneC();

                if (customers.Count() > 0)
                {
                    response.Data = customers;
                }
                else
                {
                    response.Success = false;
                    response.Message = "Does not have any customer.";
                }

                //activiti log
                _customerActivityService.InsertActivity(customer, "PublicStore.1C.ExportCustomers.LogOut", "1C Exporting customers end.");
            }
            return Json(response);
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
