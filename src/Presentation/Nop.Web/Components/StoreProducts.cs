using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Infrastructure.Cache;

namespace Nop.Web.Components
{
	public class StoreProductsViewComponent : NopViewComponent
	{
		private readonly CatalogSettings _catalogSettings;
		private readonly IAclService _aclService;
		private readonly IOrderReportService _orderReportService;
		private readonly IProductModelFactory _productModelFactory;
		private readonly IProductService _productService;
		private readonly IStaticCacheManager _cacheManager;
		private readonly IShippingService _shippingService;
		private readonly IStoreContext _storeContext;
		private readonly IStoreMappingService _storeMappingService;

		public StoreProductsViewComponent(CatalogSettings catalogSettings,
			IAclService aclService,
			IOrderReportService orderReportService,
			IProductModelFactory productModelFactory,
			IProductService productService,
			IStaticCacheManager cacheManager,
			IShippingService shippingService,
			IStoreContext storeContext,
			IStoreMappingService storeMappingService)
		{
			this._catalogSettings = catalogSettings;
			this._aclService = aclService;
			this._orderReportService = orderReportService;
			this._productModelFactory = productModelFactory;
			this._productService = productService;
			this._shippingService = shippingService;
			this._cacheManager = cacheManager;
			this._storeContext = storeContext;
			this._storeMappingService = storeMappingService;
		}

		public IViewComponentResult Invoke(int addressId)
		{
			int warehouseId = _shippingService.GetAllWarehouses().FirstOrDefault(x => x.AddressId == addressId)?.Id ?? 0;

			var products = _productService.SearchProducts(
				storeId: _storeContext.CurrentStore.Id,
				visibleIndividuallyOnly: true,
				warehouseId: warehouseId,
				orderBy: ProductSortingEnum.CreatedOn,
				pageSize: 6).ToList();


			//ACL and store mapping
			products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
			//availability dates
			products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();

			if (!products.Any())
				return Content("");

			//prepare model
			var model = _productModelFactory.PrepareProductOverviewModels(products, true, true, null).ToList();
			ViewBag.Prefix = "warehouse";//prefix for backinstock button
            ViewBag.AddressId = addressId;
            return View(model);
		}
	}
}
