using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Components
{
	public class SimilarProductsBlockViewComponent : NopViewComponent
	{
		private readonly IAclService _aclService;
		private readonly IProductModelFactory _productModelFactory;
		private readonly IProductService _productService;
		private readonly IStoreContext _storeContext;
		private readonly IStoreMappingService _storeMappingService;

		public SimilarProductsBlockViewComponent(IAclService aclService,
			IProductModelFactory productModelFactory,
			IProductService productService,
			IStoreContext storeContext,
			IStoreMappingService storeMappingService)
		{
			this._aclService = aclService;
			this._productModelFactory = productModelFactory;
			this._productService = productService;
			this._storeContext = storeContext;
			this._storeMappingService = storeMappingService;
		}

		public IViewComponentResult Invoke(string makeCode, int productId = 0)
		{
			IEnumerable<Product> products = _productService.SearchProducts(
				storeId: _storeContext.CurrentStore.Id,
				orderBy: ProductSortingEnum.CreatedOn);

            ViewBag.ProductName = products.FirstOrDefault(x => x.Id == productId)?.Name;

            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p));
			//availability dates
			products = products.Where(p => _productService.ProductIsAvailable(p) && p.MakeCode == makeCode && p.Id != productId);

			if (!products.Any())
				return Content("");

			//prepare model
			var model = _productModelFactory.PrepareProductOverviewModels(products, true, true, 250, true);
            
            var distinctColors = model.Select(x => x.SpecificationAttributeModels?.FirstOrDefault(u => u.SpecificationAttributeName.ToLower() == "цвет")?.ValueRaw).Distinct(); //(from product in model
            IEnumerable<ProductOverviewModel> newModel = new ProductOverviewModel[] { };
            foreach (var product in model)
            {
                var color = product.SpecificationAttributeModels?.FirstOrDefault(u => u.SpecificationAttributeName.ToLower() == "цвет")?.ValueRaw;
                if (!string.IsNullOrEmpty(color) && distinctColors.Contains(color))
                {
                    newModel.Append(product);
                    distinctColors = distinctColors.Where(x => x != color);
                }
            }
            model = newModel;
			ViewBag.Prefix = "similar";//prefix for backinstock button
			return View(model);
		}
	}
}
