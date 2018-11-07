using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;

namespace Nop.Web.Controllers
{
    public partial class BackInStockSubscriptionController : BasePublicController
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly IWorkContext _workContext;
		private readonly ICategoryService _categoryService;

        #endregion

        #region Ctor

        public BackInStockSubscriptionController(CatalogSettings catalogSettings,
            CustomerSettings customerSettings,
            IBackInStockSubscriptionService backInStockSubscriptionService,
            ILocalizationService localizationService,
            IProductService productService,
            IStoreContext storeContext,
			IPriceFormatter priceFormatter,
			IPictureService pictureService,
            IUrlRecordService urlRecordService,
			ICategoryService categoryService,
            IWorkContext workContext)
        {
			this._categoryService = categoryService;
            this._catalogSettings = catalogSettings;
            this._customerSettings = customerSettings;
            this._backInStockSubscriptionService = backInStockSubscriptionService;
            this._localizationService = localizationService;
            this._productService = productService;
			this._pictureService = pictureService;
			this._priceFormatter = priceFormatter;
            this._storeContext = storeContext;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
        }

		#endregion

		#region Methods

		// Product details page > back in stock subscribe
		public virtual IActionResult SubscribePopup(int productId)
		{
			var product = _productService.GetProductById(productId);
			if (product == null || product.Deleted)
				throw new ArgumentException("No product found with the specified id");

			BackInStockSubscribeModel model = new BackInStockSubscribeModel();

			model = new BackInStockSubscribeModel
			{
				ProductId = product.Id,
				ImageUrl = _pictureService.GetPictureUrl(product.ProductPictures.FirstOrDefault()?.Picture),
				//ProductCategoryName = product.ProductCategories.FirstOrDefault()?.Category.ParentCategoryId == null ? product.ProductCategories.FirstOrDefault()?.Category.Name : _categoryService.GetCategoryById(product.ProductCategories.FirstOrDefault()?.Category.ParentCategoryId ?? 0).Name,
				ProductPrice = _priceFormatter.FormatPrice(product.Price),
				ProductName = _localizationService.GetLocalized(product, x => x.Name),
				ProductSeName = _urlRecordService.GetSeName(product),
				IsCurrentCustomerRegistered = _workContext.CurrentCustomer.IsRegistered(),
				MaximumBackInStockSubscriptions = _catalogSettings.MaximumBackInStockSubscriptions,
				CurrentNumberOfBackInStockSubscriptions = _backInStockSubscriptionService
				.GetAllSubscriptionsByCustomerId(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, 0, 1)
				.TotalCount
			};
			if (product.BackorderMode == BackorderMode.NoBackorders &&
				_productService.GetTotalStockQuantity(product) <= 0)
			{
				//out of stock
				model.SubscriptionAllowed = true;
				model.AlreadySubscribed = _backInStockSubscriptionService
					.FindSubscription(_workContext.CurrentCustomer.Id, product.Id, _storeContext.CurrentStore.Id) != null;
			}
			return PartialView(model);
		}

        [HttpPost]
        public virtual IActionResult SubscribePopupPOST(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                throw new ArgumentException("No product found with the specified id");

            if (!_workContext.CurrentCustomer.IsRegistered())
                return Content(_localizationService.GetResource("BackInStockSubscriptions.OnlyRegistered"));

            if (product.BackorderMode == BackorderMode.NoBackorders &&	
                _productService.GetTotalStockQuantity(product) <= 0)
            {
                //out of stock
                var subscription = _backInStockSubscriptionService
                    .FindSubscription(_workContext.CurrentCustomer.Id, product.Id, _storeContext.CurrentStore.Id);
                if (subscription != null)
                {
                    //subscription already exists
                    //unsubscribe
                    _backInStockSubscriptionService.DeleteSubscription(subscription);

                    return Json(new
                    {
                        result = "Unsubscribed"
                    });
                }

                //subscription does not exist
                //subscribe
                if (_backInStockSubscriptionService
                    .GetAllSubscriptionsByCustomerId(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, 0, 1)
                    .TotalCount >= _catalogSettings.MaximumBackInStockSubscriptions)
                {
                    return Json(new
                    {
                        result = string.Format(_localizationService.GetResource("BackInStockSubscriptions.MaxSubscriptions"), _catalogSettings.MaximumBackInStockSubscriptions)
                    });
                }
                subscription = new BackInStockSubscription
                {
                    Customer = _workContext.CurrentCustomer,
                    Product = product,
                    StoreId = _storeContext.CurrentStore.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                _backInStockSubscriptionService.InsertSubscription(subscription);

                return Json(new
                {
                    result = "Subscribed"
                });
            }

            //subscription not possible
            return Content(_localizationService.GetResource("BackInStockSubscriptions.NotAllowed"));
        }

        // My account / Back in stock subscriptions
        public virtual IActionResult CustomerSubscriptions(int? pageNumber)
        {
            if (_customerSettings.HideBackInStockSubscriptionsTab)
            {
                return RedirectToRoute("CustomerInfo");
            }

            var pageIndex = 0;
            if (pageNumber > 0)
            {
                pageIndex = pageNumber.Value - 1;
            }
            var pageSize = 10;

            var customer = _workContext.CurrentCustomer;
            var list = _backInStockSubscriptionService.GetAllSubscriptionsByCustomerId(customer.Id,
                _storeContext.CurrentStore.Id, pageIndex, pageSize);

            var model = new CustomerBackInStockSubscriptionsModel();

            foreach (var subscription in list)
            {
                var product = subscription.Product;

                if (product != null)
                {
                    var subscriptionModel = new CustomerBackInStockSubscriptionsModel.BackInStockSubscriptionModel
                    {
                        Id = subscription.Id,
                        ProductId = product.Id,
                        ProductName = _localizationService.GetLocalized(product, x => x.Name),
                        SeName = _urlRecordService.GetSeName(product),
                    };
                    model.Subscriptions.Add(subscriptionModel);
                }
            }

            model.PagerModel = new PagerModel
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "CustomerBackInStockSubscriptionsPaged",
                UseRouteLinks = true,
                RouteValues = new BackInStockSubscriptionsRouteValues { pageNumber = pageIndex }
            };

            return View(model);
        }

        [HttpPost, ActionName("CustomerSubscriptions")]
        public virtual IActionResult CustomerSubscriptionsPOST(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("biss", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("biss", "").Trim();
                    if (int.TryParse(id, out int subscriptionId))
                    {
                        var subscription = _backInStockSubscriptionService.GetSubscriptionById(subscriptionId);
                        if (subscription != null && subscription.CustomerId == _workContext.CurrentCustomer.Id)
                        {
                            _backInStockSubscriptionService.DeleteSubscription(subscription);
                        }
                    }
                }
            }

            return RedirectToRoute("CustomerBackInStockSubscriptions");
        }

        #endregion
    }
}