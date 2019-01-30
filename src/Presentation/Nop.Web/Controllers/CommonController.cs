using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Factories;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Framework.Themes;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;

namespace Nop.Web.Controllers
{
    public partial class CommonController : BasePublicController
    {
        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IProductModelFactory _productModelFactory;
        private readonly CommonSettings _commonSettings;
        private readonly Areas.Admin.Factories.ILanguageModelFactory _languageModelFactory;
        private readonly ICommonModelFactory _commonModelFactory;
		private readonly IAddressService _addressService;
		private readonly ICurrencyService _currencyService;
		private readonly IShippingService _shippingService;
		private readonly ICustomerActivityService _customerActivityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IStoreContext _storeContext;
        private readonly IThemeContext _themeContext;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IWebHelper _webHelper;
        private readonly ILocationService _locationService;
        
        #endregion
        
        #region Ctor

        public CommonController(CaptchaSettings captchaSettings,
            CommonSettings commonSettings,
            CatalogSettings catalogSettings,
            IPictureService pictureService,
            IProductService productService,
            IProductModelFactory productModelFactory,
            Areas.Admin.Factories.ILanguageModelFactory languageModelFactory,
            ICommonModelFactory commonModelFactory,
            ICurrencyService currencyService,
			IAddressService addressService,
			ICustomerActivityService customerActivityService,
            IGenericAttributeService genericAttributeService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILogger logger,
            IStoreContext storeContext,
            IThemeContext themeContext,
            IVendorService vendorService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            StoreInformationSettings storeInformationSettings,
            VendorSettings vendorSettings,
			IShippingService shippingService,
            IWebHelper webHelper,
            ILocationService locationService)
        {
            this._pictureService = pictureService;
            this._catalogSettings = catalogSettings;
            this._productModelFactory = productModelFactory;
            this._productService = productService;
            this._languageModelFactory = languageModelFactory;
            this._captchaSettings = captchaSettings;
            this._commonSettings = commonSettings;
            this._commonModelFactory = commonModelFactory;
            this._currencyService = currencyService;
            this._customerActivityService = customerActivityService;
            this._genericAttributeService = genericAttributeService;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._logger = logger;
            this._storeContext = storeContext;
			this._addressService = addressService;
			this._themeContext = themeContext;
            this._vendorService = vendorService;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._vendorSettings = vendorSettings;
			this._shippingService = shippingService;
            this._webHelper = webHelper;
            this._locationService = locationService;
		}

		#endregion

		#region Methods

		public dynamic GetWishlistCount()
		{
			var customer = _workContext.CurrentCustomer;
			var WishlistItems = customer.ShoppingCartItems
				  .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
				  .LimitPerStore(_storeContext.CurrentStore.Id)
				  .Sum(item => item.Quantity);
			return WishlistItems;
		}

		//page not found
		public virtual IActionResult PageNotFound()
        {
            if (_commonSettings.Log404Errors)
            {
                var statusCodeReExecuteFeature = HttpContext?.Features?.Get<IStatusCodeReExecuteFeature>();
                //TODO add locale resource
                _logger.Error($"Error 404. The requested page ({statusCodeReExecuteFeature?.OriginalPath}) was not found", 
                    customer: _workContext.CurrentCustomer);
            }

            Response.StatusCode = 404;
            Response.ContentType = "text/html";

            return View();
        }
        
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult SetLanguage(int langid, string returnUrl = "")
        {
            var language = _languageService.GetLanguageById(langid);
            if (!language?.Published ?? false)
                language = _workContext.WorkingLanguage;

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            //language part in URL
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                //remove current language code if it's already localized URL
                if (returnUrl.IsLocalizedUrl(this.Request.PathBase, true, out Language _))
                    returnUrl = returnUrl.RemoveLanguageSeoCodeFromUrl(this.Request.PathBase, true);

                //and add code of passed language
                returnUrl = returnUrl.AddLanguageSeoCodeToUrl(this.Request.PathBase, true, language);
            }

            _workContext.WorkingLanguage = language;

            return Redirect(returnUrl);
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult SetCurrency(int customerCurrency, string returnUrl = "")
        {
            var currency = _currencyService.GetCurrencyById(customerCurrency);
            if (currency != null)
                _workContext.WorkingCurrency = currency;

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            return Redirect(returnUrl);
        }
        
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult SetTaxType(int customerTaxType, string returnUrl = "")
        {
            var taxDisplayType = (TaxDisplayType)Enum.ToObject(typeof(TaxDisplayType), customerTaxType);
            _workContext.TaxDisplayType = taxDisplayType;

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            return Redirect(returnUrl);
        }

        //contact us page
        [HttpsRequirement(SslRequirement.Yes)]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        public virtual IActionResult ContactUs()
        {
            var model = new ContactUsModel();
            model = _commonModelFactory.PrepareContactUsModel(model, false);
            return View(model);
        }

        [HttpPost, ActionName("ContactUs")]
        [PublicAntiForgery]
        [ValidateCaptcha]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        public virtual IActionResult ContactUsSend(ContactUsModel model, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            model = _commonModelFactory.PrepareContactUsModel(model, true);

            if (ModelState.IsValid)
            {
                var subject = _commonSettings.SubjectFieldOnContactUsForm ? model.Subject : null;
                var body = Core.Html.HtmlHelper.FormatText(model.Enquiry, false, true, false, false, false, false);

                _workflowMessageService.SendContactUsMessage(_workContext.WorkingLanguage.Id,
                    model.Email.Trim(), model.FullName, subject, body);

                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("ContactUs.YourEnquiryHasBeenSent");

                //activity log
                _customerActivityService.InsertActivity("PublicStore.ContactUs", 
                    _localizationService.GetResource("ActivityLog.PublicStore.ContactUs"));

                return View(model);
            }

            return View(model);
        }

        //contact vendor page
        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult ContactVendor(int vendorId)
        {
            if (!_vendorSettings.AllowCustomersToContactVendors)
                return RedirectToRoute("HomePage");

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null || !vendor.Active || vendor.Deleted)
                return RedirectToRoute("HomePage");

            var model = new ContactVendorModel();
            model = _commonModelFactory.PrepareContactVendorModel(model, vendor, false);
            return View(model);
        }

        [HttpPost, ActionName("ContactVendor")]
        [PublicAntiForgery]
        [ValidateCaptcha]
        public virtual IActionResult ContactVendorSend(ContactVendorModel model, bool captchaValid)
        {
            if (!_vendorSettings.AllowCustomersToContactVendors)
                return RedirectToRoute("HomePage");

            var vendor = _vendorService.GetVendorById(model.VendorId);
            if (vendor == null || !vendor.Active || vendor.Deleted)
                return RedirectToRoute("HomePage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            model = _commonModelFactory.PrepareContactVendorModel(model, vendor, true);

            if (ModelState.IsValid)
            {
                var subject = _commonSettings.SubjectFieldOnContactUsForm ? model.Subject : null;
                var body = Core.Html.HtmlHelper.FormatText(model.Enquiry, false, true, false, false, false, false);

                _workflowMessageService.SendContactVendorMessage(vendor, _workContext.WorkingLanguage.Id,
                    model.Email.Trim(), model.FullName, subject, body);

                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("ContactVendor.YourEnquiryHasBeenSent");

                return View(model);
            }

            return View(model);
        }

        //sitemap page
        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult Sitemap(SitemapPageModel pageModel)
        {
            if (!_commonSettings.SitemapEnabled)
                return RedirectToRoute("HomePage");

            var model = _commonModelFactory.PrepareSitemapModel(pageModel);
            return View(model);
        }

        //SEO sitemap page
        [HttpsRequirement(SslRequirement.No)]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        public virtual IActionResult SitemapXml(int? id)
        {
            if (!_commonSettings.SitemapEnabled)
                return RedirectToRoute("HomePage");

            var siteMap = _commonModelFactory.PrepareSitemapXml(id);
            return Content(siteMap, "text/xml");
        }

        public virtual IActionResult SetStoreTheme(string themeName, string returnUrl = "")
        {
            _themeContext.WorkingThemeName = themeName;

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            return Redirect(returnUrl);
        }

        [HttpPost]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult EuCookieLawAccept()
        {
            if (!_storeInformationSettings.DisplayEuCookieLawWarning)
                //disabled
                return Json(new { stored = false });

            //save setting
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, NopCustomerDefaults.EuCookieLawAcceptedAttribute, true, _storeContext.CurrentStore.Id);
            return Json(new { stored = true });
        }

        //robots.txt file
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult RobotsTextFile()
        {
            var robotsFileContent = _commonModelFactory.PrepareRobotsTextFile();
            return Content(robotsFileContent, MimeTypes.TextPlain);
        }

        public virtual IActionResult GenericUrl()
        {
            //seems that no entity was found
            return InvokeHttp404();
        }

        //store is closed
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        public virtual IActionResult StoreClosed()
        {
            return View();
        }

        //helper method to redirect users. Workaround for GenericPathRoute class where we're not allowed to do it
        public virtual IActionResult InternalRedirect(string url, bool permanentRedirect)
        {
            //ensure it's invoked from our GenericPathRoute class
            if (HttpContext.Items["nop.RedirectFromGenericPathRoute"] == null ||
                !Convert.ToBoolean(HttpContext.Items["nop.RedirectFromGenericPathRoute"]))
            {
                url = Url.RouteUrl("HomePage");
                permanentRedirect = false;
            }

            //home page
            if (string.IsNullOrEmpty(url))
            {
                url = Url.RouteUrl("HomePage");
                permanentRedirect = false;
            }

            //prevent open redirection attack
            if (!Url.IsLocalUrl(url))
            {
                url = Url.RouteUrl("HomePage");
                permanentRedirect = false;
            }

            if (permanentRedirect)
                return RedirectPermanent(url);

            return Redirect(url);
        }

		//our stores page
		[HttpsRequirement(SslRequirement.Yes)]
		public virtual IActionResult OurStores()
		{
			var warehouses = _shippingService.GetAllWarehouses();
			WarehouseInfoModel model = new WarehouseInfoModel
			{
				Cities = new List<string>(),
				Warehouses = warehouses
			};
			var addresses = new List<Address>();
			foreach (var warehouse in warehouses)
			{
				addresses.Add(_addressService.GetAddressById(warehouse.AddressId));
				var address = _addressService.GetAddressById(warehouse?.AddressId ?? 0);
				if (address != null && !model.Cities.Contains(address.City))
				{
					model.Cities.Add(address.City);
				}
			}
			model.Addresses = addresses.Where(x => x != null).ToList();

			return View(model);
		}	

		//our stores page
		[HttpsRequirement(SslRequirement.Yes)]
		public virtual IActionResult CityMap(string city)
		{
			var warehouses = _shippingService.GetAllWarehouses();
			var addresses = new List<Address>();
			foreach (var warehouse in warehouses)
			{
				addresses.Add(_addressService.GetAddressById(warehouse.AddressId));
			}

			var cityAdresses = addresses.Where(x => x?.City?.ToLower() == city.ToLower()).ToList();
			var warehousesInCity = new List<Warehouse>();
			foreach (var cityAddress in cityAdresses)
			{
				warehousesInCity.AddRange(warehouses.Where(x => x.AddressId == cityAddress.Id));
			}
            var viewWarehouseModelList = new List<ViewWarehouseModel>();
            warehousesInCity.ForEach(x =>
            {
                viewWarehouseModelList.Add(new ViewWarehouseModel()
                {
                    Name = x.Name,
                    WorkTime = x.AdminComment,
                    AddressId = x.AddressId,
                    Pictures = _shippingService.GetWarehousePictures(x.Id).Select(u => new ViewWarehouseModel.WarehousePicture()
                    {
                        PictureUrl = _pictureService.GetPictureUrl(u.PictureId)
                    }).ToList()
                });
            });
            viewWarehouseModelList.Where(x => x.Pictures.Count == 0).ToList().ForEach(x =>
            {
                x.Pictures.Add(new ViewWarehouseModel.WarehousePicture()
                {
                    PictureUrl = _pictureService.GetPictureUrl(0)
                });
            });
			var model = new CityMapModel
			{
				Warehouses = warehousesInCity,
                WarehouseViewModels = viewWarehouseModelList,
				Addresses = cityAdresses,
				Name = city
			};
			return View(model);
		}	  	

		//store info page
		[HttpsRequirement(SslRequirement.Yes)]
		public virtual IActionResult StoreInfo(int addressId)
		{
			var address = _addressService.GetAddressById(addressId);

			var city = address.City;
			var warehouses = _shippingService.GetAllWarehouses();
			var addresses = new List<Address>();
			foreach (var warehouse in warehouses)
			{
				addresses.Add(_addressService.GetAddressById(warehouse.AddressId));
			}

			var cityAdresses = addresses.Where(x => x?.City?.ToLower() == city.ToLower()).Take(3).ToList();
            var warehousesInCity = new List<Warehouse>();
            foreach (var cityAddress in cityAdresses)
            {
                warehousesInCity.AddRange(warehouses.Where(x => x.AddressId == cityAddress.Id));
            }
            var viewWarehouseModelList = new List<ViewWarehouseModel>();
            warehousesInCity.ForEach(x =>
            {
                viewWarehouseModelList.Add(new ViewWarehouseModel()
                {

                    Name = x.Name,
                    WorkTime = x.AdminComment,
                    AddressId = x.AddressId,
                    Pictures = _shippingService.GetWarehousePictures(x.Id).Select(u => new ViewWarehouseModel.WarehousePicture()
                    {
                        PictureUrl = _pictureService.GetPictureUrl(u.PictureId)
                    }).ToList()
                });
            });
            viewWarehouseModelList.Where(x => x.Pictures.Count == 0).ToList().ForEach(x =>
            {
                x.Pictures.Add(new ViewWarehouseModel.WarehousePicture()
                {
                    PictureUrl = _pictureService.GetPictureUrl(0)
                });
            });
            var model = new StoreInfoModel
			{
				Address = address,
                WarehouseViewModels = viewWarehouseModelList,
                OtherStores = cityAdresses,
				Warehouses = warehouses
			};
			return View(model);
		}

        #endregion

        public JsonResult CityJson()
        {
            var location = _locationService.GetLocation(_webHelper.GetCurrentIpAddress());
            return Json(_localizationService.GetResource(string.Format("cities.{0}", location?.city)));
        }

        public JsonResult CitySuggestionsJson(string query)
        {
            var language = _languageService.GetLanguageById(_workContext.WorkingLanguage.Id, false);

            //prepare model
            var model = _languageModelFactory.PrepareLocaleResourceListModel(new LocaleResourceSearchModel() { SearchResourceValue = query,
                SearchResourceName = "cities." }, language);
            if (_workContext.WorkingLanguage.Id == 4)
            {
                return Json(model.Data.Where(x => x.Id >= 27081 && x.Id <= 27437).Select(x => new { value = x.Name, label = x.Value })); //ids of cities (translates)
            }
            else
            {
                return Json(model.Data.Where(x => x.Id >= 26617 && x.Id <= 26973).Select(x => new { value = x.Name, label = x.Value })); //ids of cities (translates)
            }
        }

        [HttpPost]
        public virtual dynamic ChangeRecommendation(int ReviewId, bool? WillRecommend)
        {
            var product = _productService.GetProductById(1);
            if (product == null || product.ProductReviews.FirstOrDefault(x => x.Id == ReviewId && x.CustomerId == _workContext.CurrentCustomer.Id) == null)
            {
                return new { success = false };
            }
            product.ProductReviews.FirstOrDefault(x => x.Id == ReviewId && x.CustomerId == _workContext.CurrentCustomer.Id).WillRecommend = WillRecommend;
            _productService.UpdateProductReviewTotals(product);
            return new { success = true };
        }

        public virtual IActionResult StoreReviews()
        {
            var product = _productService.GetProductById(1);
            if (product == null)
                return RedirectToRoute("HomePage");

            var model = new ProductReviewsModel();
            model = _productModelFactory.PrepareProductReviewsModel(model, product);

            //default value
            model.AddProductReview.Rating = _catalogSettings.DefaultProductRatingValue;

            //default value for all additional review types
            if (model.ReviewTypeList.Count > 0)
                foreach (var additionalProductReview in model.AddAdditionalProductReviewList)
                {
                    additionalProductReview.Rating = additionalProductReview.IsRequired ? _catalogSettings.DefaultProductRatingValue : 0;
                }

            ViewBag.CustomerId = _workContext.CurrentCustomer.Id;

            return View(model);
        }

        [HttpPost, ActionName("StoreReviews")]
        [PublicAntiForgery]
        [ValidateCaptcha]
        public virtual dynamic StoreReviewsAdd(ProductReviewsModel model, bool captchaValid)
        {
            var product = _productService.GetProductById(1);
            if (product == null)
                return RedirectToRoute("HomePage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnProductReviewPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));
            }

            if (ModelState.IsValid)
            {
                //save review
                var rating = model.AddProductReview.Rating;
                if (rating < 1 || rating > 5)
                    rating = _catalogSettings.DefaultProductRatingValue;
                var isApproved = !_catalogSettings.ProductReviewsMustBeApproved;

                var productReview = new ProductReview
                {
                    ProductId = product.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    Title = model.AddProductReview.Title,
                    ReviewText = model.AddProductReview.ReviewText,
                    Rating = rating,
                    ConsultationRating = model.AddProductReview.ConsultationRating,
                    DeliveryRating = model.AddProductReview.DeliveryRating,
                    InstallationRating = model.AddProductReview.InstallationRating,
                    OfflineOrderRating = model.AddProductReview.OfflineOrderRating,
                    OnlineOrderRating = model.AddProductReview.OnlineOrderRating,
                    WillRecommend = model.AddProductReview.WillRecommend,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    IsApproved = isApproved,
                    CreatedOnUtc = DateTime.UtcNow,
                    StoreId = _storeContext.CurrentStore.Id,
                };

                product.ProductReviews.Add(productReview);

                //add product review and review type mapping                
                foreach (var additionalReview in model.AddAdditionalProductReviewList)
                {
                    var additionalProductReview = new ProductReviewReviewTypeMapping
                    {
                        ProductReviewId = productReview.Id,
                        ReviewTypeId = additionalReview.ReviewTypeId,
                        Rating = additionalReview.Rating
                    };
                    productReview.ProductReviewReviewTypeMappingEntries.Add(additionalProductReview);
                }

                //update product totals
                _productService.UpdateProductReviewTotals(product);

                //notify store owner
                if (_catalogSettings.NotifyStoreOwnerAboutNewProductReviews)
                    _workflowMessageService.SendProductReviewNotificationMessage(productReview, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                _customerActivityService.InsertActivity("PublicStore.AddProductReview",
                    string.Format(_localizationService.GetResource("ActivityLog.PublicStore.AddProductReview"), product.Name), product);


                model = _productModelFactory.PrepareProductReviewsModel(model, product);
                model.AddProductReview.Title = null;
                model.AddProductReview.ReviewText = null;

                model.AddProductReview.SuccessfullyAdded = true;
                if (!isApproved)
                    model.AddProductReview.Result = _localizationService.GetResource("Reviews.SeeAfterApproving");
                else
                    model.AddProductReview.Result = _localizationService.GetResource("Reviews.SuccessfullyAdded");

                return new { success = model.AddProductReview.SuccessfullyAdded };
            }
            else
            {
                return new { error = true, message = ModelState };
            }

        }
    }
}