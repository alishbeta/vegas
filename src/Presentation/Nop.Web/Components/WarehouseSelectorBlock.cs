using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Framework.Components;
using System.Linq;

namespace Nop.Web.Components
{
    public class WarehouseSelectorBlockViewComponent : NopViewComponent
    {
        private readonly IShippingService _shippingService;

        public WarehouseSelectorBlockViewComponent(IShippingService shippingService)
        {
            this._shippingService = shippingService;
        }

        public IViewComponentResult Invoke()
        {
            var model = _shippingService.GetActiveWarehouses();
            return View(model);
        }
    }
}
