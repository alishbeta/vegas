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
	public class SimilarProductsSizesBlockViewComponent : NopViewComponent
	{
		private readonly IAclService _aclService;
		private readonly IProductModelFactory _productModelFactory;
		private readonly IProductService _productService;
		private readonly IStoreContext _storeContext;
		private readonly IStoreMappingService _storeMappingService;
        private readonly ISpecificationAttributeService _specificationAttributeService;

		public SimilarProductsSizesBlockViewComponent(IAclService aclService,
			IProductModelFactory productModelFactory,
			IProductService productService,
            ISpecificationAttributeService specificationAttributeService,
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

		public IViewComponentResult Invoke(string makeCode, string colorName, int productId = 0, bool isSleepSizes = false)
		{
            if (string.IsNullOrEmpty(makeCode))
            {
                return Content("");
            }
			IEnumerable<Product> products = _productService.SearchProducts(
				storeId: _storeContext.CurrentStore.Id,
				orderBy: ProductSortingEnum.CreatedOn);
            
            var product = products.FirstOrDefault(x => x.Id == productId);
            if (product == null)
            {
                return Content("");
            }

            ViewBag.ProductName = product.Name;

            //prepare model
            var model = _specificationAttributeService.GetSimilarProductSizes(makeCode, colorName, productId, isSleepSizes);
            return View(model);
		}
	}
}
