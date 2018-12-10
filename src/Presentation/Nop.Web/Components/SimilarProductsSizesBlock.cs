﻿using System.Collections.Generic;
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
			var products = _productService.SearchProducts(
				storeId: _storeContext.CurrentStore.Id,
				orderBy: ProductSortingEnum.CreatedOn
				).ToList();

            ViewBag.ProductName = products.FirstOrDefault(x => x.Id == productId)?.Name;

            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
			//availability dates
			products = products.Where(p => _productService.ProductIsAvailable(p) && p.MakeCode == makeCode && p.Id != productId).ToList();

			if (!products.Any())
				return Content("");

			//prepare model
			var productOverviewModels = _productModelFactory.PrepareProductOverviewModels(products, true, true, null, true).ToList();
            List<SimilarProductSizesModel> model = new List<SimilarProductSizesModel>();
            if (isSleepSizes)
            {
                model = productOverviewModels.Select(x => new SimilarProductSizesModel()
                {
                    height = null,
                    length = x.SleepLength.ToString("#.##"),
                    productUrl = Url.RouteUrl("Product", new { productId = x.Id, x.SeName }),
                    width = x.SleepWidth.ToString("#.##")
                }).ToList();
            }
            else
            {
                model = productOverviewModels.Select(x => new SimilarProductSizesModel()
                {
                    height = x.Height.ToString("#.##"),
                    length = x.Length.ToString("#.##"),
                    productUrl = Url.RouteUrl("Product", new { productId = x.Id, x.SeName }),
                    width = x.Width.ToString("#.##")
                }).ToList();
            }
			return View(model);
		}
	}
}