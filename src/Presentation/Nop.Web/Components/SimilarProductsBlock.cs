using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
	public class SimilarProductsBlockViewComponent : NopViewComponent
	{
		private readonly IAclService _aclService;
		private readonly IProductModelFactory _productModelFactory;
		private readonly IProductService _productService;
		private readonly IStoreContext _storeContext;
		private readonly IStoreMappingService _storeMappingService;
        private readonly ISpecificationAttributeService _specificationAttributeService;

		public SimilarProductsBlockViewComponent(IAclService aclService,
			IProductModelFactory productModelFactory,
            ISpecificationAttributeService specificationAttributeService,
			IProductService productService,
			IStoreContext storeContext,
			IStoreMappingService storeMappingService)
		{
            this._specificationAttributeService = specificationAttributeService;
			this._aclService = aclService;
			this._productModelFactory = productModelFactory;
			this._productService = productService;
			this._storeContext = storeContext;
			this._storeMappingService = storeMappingService;
		}

		public IViewComponentResult Invoke(string makeCode, string colorName, string productName, int productId = 0)
		{
            if (string.IsNullOrEmpty(colorName) || string.IsNullOrEmpty(makeCode))
            {
                return Content("");
            }
            IEnumerable<Product> products = _productService.SearchProducts(
				storeId: _storeContext.CurrentStore.Id,
				orderBy: ProductSortingEnum.CreatedOn);

            ViewBag.ProductName = productName;

            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p));
			//availability dates
			products = products.Where(p => _productService.ProductIsAvailable(p) && p.MakeCode == makeCode && p.Id != productId);

			if (!products.Any())
				return Content("");

            var productIds = _specificationAttributeService.GetSimilarProductIdsByColor(makeCode, colorName, productId);

            products = products.Where(x => productIds.Contains(x.Id));

			//prepare model
			var model = _productModelFactory.PrepareProductOverviewModels(products, true, true, 250, false);
			ViewBag.Prefix = "similar";//prefix for backinstock button
			return View(model);
		}
	}
}
