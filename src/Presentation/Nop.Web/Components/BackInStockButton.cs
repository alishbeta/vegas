using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Components
{
    public class BackInStockButtonViewComponent : NopViewComponent
    {

        public BackInStockButtonViewComponent()
        {
        }

        public IViewComponentResult Invoke(int productId, string prefix = "")
        {
			var model = new BackInStockButtonModel()
			{
					Prefix = prefix,
					ProductId = productId
			};
            return View(model);
        }
    }
}
