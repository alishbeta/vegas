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
using Nop.Web.Models.Common;

namespace Nop.Web.Components
{
	public class SimilarProductsSizesBlockViewComponent : NopViewComponent
	{
		private readonly IAclService _aclService;
		private readonly IProductModelFactory _productModelFactory;
		private readonly IProductService _productService;
		private readonly IStoreContext _storeContext;
		private readonly IStoreMappingService _storeMappingService;

		public SimilarProductsSizesBlockViewComponent(IAclService aclService,
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

		public IViewComponentResult Invoke(string makeCode, int productId = 0, bool isSleepSizes = false)
		{
			IEnumerable<Product> products = _productService.SearchProducts(
				storeId: _storeContext.CurrentStore.Id,
				orderBy: ProductSortingEnum.CreatedOn);

            
            var product = products.FirstOrDefault(x => x.Id == productId);
            if (product == null)
            {
                return Content("");
            }
            ViewBag.ProductName = product.Name;

            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p));
			//availability dates
			products = products.Where(p => _productService.ProductIsAvailable(p) && p.MakeCode == makeCode && p.Id != productId);

			if (!products.Any())
				return Content("");

			//prepare model
			var productOverviewModels = _productModelFactory.PrepareProductOverviewModels(products, true, true, null, true);
            var productOverviewModel = _productModelFactory.PrepareProductOverviewModels(new Product[] { product }, false, false, null, true).FirstOrDefault();
            var color = productOverviewModel.SpecificationAttributeModels?.FirstOrDefault(x => x.SpecificationAttributeName.ToLower() == "цвет")?.ValueRaw;
            //var color = productOverviewModel.FirstOrDefault(x => x.SpecificationAttributeModels?.FirstOrDefault(u => u.SpecificationAttributeName.ToLower() == "цвет")).ValueRaw;
            productOverviewModels = productOverviewModels.Where(x => x.SpecificationAttributeModels?.FirstOrDefault(u => u.SpecificationAttributeName.ToLower() == "цвет")?.ValueRaw == color);
            IEnumerable<SimilarProductSizesModel> model = new SimilarProductSizesModel[] { };
            if (isSleepSizes)
            {
                model = productOverviewModels.Select(x => new SimilarProductSizesModel()
                {
                    height = null,
                    length = x.SleepLength.ToString("#.##"),
                    productUrl = Url.RouteUrl("Product", new { productId = x.Id, x.SeName }),
                    width = x.SleepWidth.ToString("#.##")
                });
            }
            else
            {
                model = productOverviewModels.Select(x => new SimilarProductSizesModel()
                {
                    height = x.Height.ToString("#.##"),
                    length = x.Length.ToString("#.##"),
                    productUrl = Url.RouteUrl("Product", new { productId = x.Id, x.SeName }),
                    width = x.Width.ToString("#.##")
                });
            }
			return View(model);
		}
	}
}
