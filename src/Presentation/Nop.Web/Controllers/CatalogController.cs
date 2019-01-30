using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Controllers
{
    public partial class CatalogController : BasePublicController
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly ICatalogModelFactory _catalogModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPermissionService _permissionService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IVendorService _vendorService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public CatalogController(CatalogSettings catalogSettings,
            IAclService aclService,
            ICatalogModelFactory catalogModelFactory,
            ICategoryService categoryService, 
            ICustomerActivityService customerActivityService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IPermissionService permissionService, 
            IProductModelFactory productModelFactory,
            IProductService productService, 
            IProductTagService productTagService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext, 
            MediaSettings mediaSettings,
            VendorSettings vendorSettings)
        {
            this._catalogSettings = catalogSettings;
            this._aclService = aclService;
            this._catalogModelFactory = catalogModelFactory;
            this._categoryService = categoryService;
            this._customerActivityService = customerActivityService;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._manufacturerService = manufacturerService;
            this._permissionService = permissionService;
            this._productModelFactory = productModelFactory;
            this._productService = productService;
            this._productTagService = productTagService;
            this._storeContext = storeContext;
            this._storeMappingService = storeMappingService;
            this._vendorService = vendorService;
            this._webHelper = webHelper;
            this._workContext = workContext;
            this._mediaSettings = mediaSettings;
            this._vendorSettings = vendorSettings;
        }

        #endregion
        
        #region Categories
        
        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult Category(string SeName, CatalogPagingFilteringModel command) //int categoryId, CatalogPagingFilteringModel command)
        {
            if (command.PageNumber == 1)
            {
                return RedirectPermanent(_webHelper.RemoveQueryString(_webHelper.GetThisPageUrl(true), "pagenumber"));
            }
            int categoryId = _categoryService.GetCategoryIdBySeName(SeName);
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null || category.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                !category.Published ||
                //ACL (access control list) 
                !_aclService.Authorize(category) ||
                //Store mapping
                !_storeMappingService.Authorize(category);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            if (notAvailable && !_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return InvokeHttp404();

            //'Continue shopping' URL
            //_genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, 
            //    NopCustomerDefaults.LastContinueShoppingPageAttribute, 
            //    _webHelper.GetThisPageUrl(false),
            //    _storeContext.CurrentStore.Id);

            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                DisplayEditLink(Url.Action("Edit", "Category", new { id = category.Id, area = AreaNames.Admin }));

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewCategory",
                string.Format(_localizationService.GetResource("ActivityLog.PublicStore.ViewCategory"), category.Name), category);

            //model
            var model = _catalogModelFactory.PrepareCategoryModel(category, command);
            
			var activeCategory = (category.ParentCategoryId != 0 ? category.ParentCategoryId : categoryId);
            ViewBag.ActiveCategory = activeCategory;
            ViewBag.ActiveSubCategory = categoryId;
			ViewBag.Prefix = "cat"; //prefix for backinstock button
            //check if products in category has sleep sizes (for CatalogFiltersSelector)
            ViewBag.HasSleepSizes = activeCategory == 19; //id of category "Кровати", wich has sleep sizes
            ViewBag.HasHeight = activeCategory == 21 || activeCategory == 19; //id of category "Кровати" "Тумбы и комоды", wich has sleep sizes
			//template
			var templateViewPath = _catalogModelFactory.PrepareCategoryTemplateViewPath(category.CategoryTemplateId);
            return View(templateViewPath, model);
        }

		[HttpsRequirement(SslRequirement.No)]
		public dynamic GetProducts(int categoryId, int pageIndex)
		{
            //include subcategories
            IList<int> categoryIds = _categoryService.GetChildCategoryIds(categoryId, _storeContext.CurrentStore.Id);
            categoryIds.Add(categoryId);

            decimal? minPrice = null, maxPrice = null;
            if (!string.IsNullOrEmpty(_webHelper.QueryString<string>("price")))
            {
                minPrice = decimal.Parse(_webHelper.QueryString<string>("price").Split('-')[0]);
                maxPrice = decimal.Parse(_webHelper.QueryString<string>("price").Split('-')[1]);
            }
            decimal? minLength = null, maxLength = null;
            if (!string.IsNullOrEmpty(_webHelper.QueryString<string>("length")))
            {
                minLength = decimal.Parse(_webHelper.QueryString<string>("length").Split('-')[0]);
                maxLength = decimal.Parse(_webHelper.QueryString<string>("length").Split('-')[1]);
            }
            decimal? minWidth = null, maxWidth = null;
            if (!string.IsNullOrEmpty(_webHelper.QueryString<string>("width")))
            {
                minWidth = decimal.Parse(_webHelper.QueryString<string>("width").Split('-')[0]);
                maxWidth = decimal.Parse(_webHelper.QueryString<string>("width").Split('-')[1]);
            }
            decimal? minHeight = null, maxHeight = null;
            if (!string.IsNullOrEmpty(_webHelper.QueryString<string>("height")))
            {
                minHeight = decimal.Parse(_webHelper.QueryString<string>("height").Split('-')[0]);
                maxHeight = decimal.Parse(_webHelper.QueryString<string>("height").Split('-')[1]);
            }
            decimal? minSleepLength = null, maxSleepLength = null;
            if (!string.IsNullOrEmpty(_webHelper.QueryString<string>("sleeplength")))
            {
                minSleepLength = decimal.Parse(_webHelper.QueryString<string>("sleeplength").Split('-')[0]);
                maxSleepLength = decimal.Parse(_webHelper.QueryString<string>("sleeplength").Split('-')[1]);
            }
            decimal? minSleepWidth = null, maxSleepWidth = null;
            if (!string.IsNullOrEmpty(_webHelper.QueryString<string>("sleepwidth")))
            {
                minSleepWidth = decimal.Parse(_webHelper.QueryString<string>("sleepwidth").Split('-')[0]);
                maxSleepWidth = decimal.Parse(_webHelper.QueryString<string>("sleepwidth").Split('-')[1]);
            }

            IEnumerable<Product> products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds,
                true,
                categoryIds: categoryIds,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false,
                priceMin: minPrice,
                priceMax: maxPrice,
                MinHeight: minHeight,
                MaxHeight: maxHeight,
                MinLength: minLength,
                MaxLength: maxLength,
                MinWidth: minWidth,
                MaxWidth: maxWidth,
                MinSleepLength: minSleepLength,
                MaxSleepLength: maxSleepLength,
                MinSleepWidth: minSleepWidth,
                MaxSleepWidth: maxSleepWidth,
                pageIndex: pageIndex - 1,
                pageSize: 32);

            //        IEnumerable<Product> products = _productService.SearchProducts(
            //storeId: _storeContext.CurrentStore.Id,
            //categoryIds: categories,
            //pageIndex: pageIndex - 1,
            //pageSize: 32);


            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p));
			//availability dates
			products = products.Where(p => _productService.ProductIsAvailable(p) && !p.Deleted && p.Published);

			if (!products.Any())
				return Content("");

			//prepare model
			var model = _productModelFactory.PrepareProductOverviewModels(products, true, true);
			return new { model };
		}

        #endregion

        #region Manufacturers

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult Manufacturer(int manufacturerId, CatalogPagingFilteringModel command)
        {
            var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);
            if (manufacturer == null || manufacturer.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                !manufacturer.Published ||
                //ACL (access control list) 
                !_aclService.Authorize(manufacturer) ||
                //Store mapping
                !_storeMappingService.Authorize(manufacturer);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            if (notAvailable && !_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return InvokeHttp404();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, 
                NopCustomerDefaults.LastContinueShoppingPageAttribute, 
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);
            
            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                DisplayEditLink(Url.Action("Edit", "Manufacturer", new { id = manufacturer.Id, area = AreaNames.Admin }));

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewManufacturer",
                string.Format(_localizationService.GetResource("ActivityLog.PublicStore.ViewManufacturer"), manufacturer.Name), manufacturer);

            //model
            var model = _catalogModelFactory.PrepareManufacturerModel(manufacturer, command);
            
            //template
            var templateViewPath = _catalogModelFactory.PrepareManufacturerTemplateViewPath(manufacturer.ManufacturerTemplateId);
            return View(templateViewPath, model);
        }

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult ManufacturerAll()
        {
            var model = _catalogModelFactory.PrepareManufacturerAllModels();
            return View(model);
        }
        
        #endregion

        #region Vendors

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult Vendor(int vendorId, CatalogPagingFilteringModel command)
        {
            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null || vendor.Deleted || !vendor.Active)
                return InvokeHttp404();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);
            
            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                DisplayEditLink(Url.Action("Edit", "Vendor", new { id = vendor.Id, area = AreaNames.Admin }));

            //model
            var model = _catalogModelFactory.PrepareVendorModel(vendor, command);

            return View(model);
        }

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult VendorAll()
        {
            //we don't allow viewing of vendors if "vendors" block is hidden
            if (_vendorSettings.VendorsBlockItemsToDisplay == 0)
                return RedirectToRoute("HomePage");

            var model = _catalogModelFactory.PrepareVendorAllModels();
            return View(model);
        }

        #endregion

        #region Product tags
        
        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult ProductsByTag(int productTagId, CatalogPagingFilteringModel command)
        {
            var productTag = _productTagService.GetProductTagById(productTagId);
            if (productTag == null)
                return InvokeHttp404();

            var model = _catalogModelFactory.PrepareProductsByTagModel(productTag, command);
            return View(model);
        }

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult ProductTagsAll()
        {
            var model = _catalogModelFactory.PrepareProductTagsAllModel();
            return View(model);
        }

        #endregion

        #region Searching

        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult Search(SearchModel model, CatalogPagingFilteringModel command)
        {
            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            if (model == null)
                model = new SearchModel();

            model = _catalogModelFactory.PrepareSearchModel(model, command);
            return View(model);
        }

        public virtual IActionResult SearchTermAutoComplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < _catalogSettings.ProductSearchTermMinimumLength)
                return Content("");

            //products
            var productNumber = _catalogSettings.ProductSearchAutoCompleteNumberOfProducts > 0 ?
                _catalogSettings.ProductSearchAutoCompleteNumberOfProducts : 10;            

            var products = _productService.SearchProducts(
                storeId: _storeContext.CurrentStore.Id,
                keywords: term,
                languageId: _workContext.WorkingLanguage.Id,
                visibleIndividuallyOnly: true,
                pageSize: productNumber);

            var showLinkToResultSearch = _catalogSettings.ShowLinkToAllResultInSearchAutoComplete && (products.TotalCount > productNumber);

            var models =  _productModelFactory.PrepareProductOverviewModels(products, false, _catalogSettings.ShowProductImagesInSearchAutoComplete, _mediaSettings.AutoCompleteSearchThumbPictureSize).ToList();
            var result = (from p in models
                    select new
                    {
                        label = p.Name,
                        producturl = Url.RouteUrl("Product", new {SeName = p.SeName}),
                        productpictureurl = p.DefaultPictureModel.ImageUrl,
                        showlinktoresultsearch = showLinkToResultSearch
                    })
                .ToList();
            return Json(result);
        }
        
        #endregion
    }
}