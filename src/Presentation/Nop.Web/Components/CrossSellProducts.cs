using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Order;

namespace Nop.Web.Components
{
    public class CrossSellProductsViewComponent : NopViewComponent
    {
        private readonly IAclService _aclService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        public CrossSellProductsViewComponent(IAclService aclService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IWorkContext workContext,
            ShoppingCartSettings shoppingCartSettings)
        {
            this._aclService = aclService;
            this._productModelFactory = productModelFactory;
            this._productService = productService;
            this._storeContext = storeContext;
            this._storeMappingService = storeMappingService;
            this._workContext = workContext;
            this._shoppingCartSettings = shoppingCartSettings;
        }

        public IViewComponentResult Invoke(int? productThumbPictureSize, int[] productIds = null)
        {
			IList<Product> products = new List<Product>();
			List<Product> result = new List<Product>();
			if (productIds == null)
			{
				var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
			
				products = _productService.GetCrosssellProductsByShoppingCart(cart, _shoppingCartSettings.CrossSellsNumber);
				//ACL and store mapping
				products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
				//availability dates
				products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();
				//visible individually
				products = products.Where(p => p.VisibleIndividually).ToList();
			}
			else
			{
				var crosSellProducts = _productService.GetCrossSellProductsByProductIds(productIds);
				foreach (var crossSell in crosSellProducts)
				{
					//validate that this product is not added to result yet
					//validate that this product is not in the cart
					if (result.Find(p => p.Id == crossSell.ProductId2) != null || productIds.Contains(crossSell.ProductId2))
						continue;

					var productToAdd = _productService.GetProductById(crossSell.ProductId2);
					//validate product
					if (productToAdd == null || productToAdd.Deleted || !productToAdd.Published)
						continue;

					//add a product to result
					result.Add(productToAdd);
				}
				products = result.ToList();
			}
            if (!products.Any())
                return Content("");

            //Cross-sell products are displayed on the shopping cart page.
            //We know that the entire shopping cart page is not refresh
            //even if "ShoppingCartSettings.DisplayCartAfterAddingProduct" setting  is enabled.
            //That's why we force page refresh (redirect) in this case
            var model = _productModelFactory.PrepareProductOverviewModels(products,
                    productThumbPictureSize: productThumbPictureSize, forceRedirectionAfterAddingToCart: true)
                .ToList();
			ViewBag.Prefix = "cross";//prefix for backinstock button
			return View(model);
        }
    }
}