using Microsoft.AspNetCore.Mvc;
using Nop.Services.Common;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Components
{
    public class OurStoresMenuViewComponent : NopViewComponent
	{
		private readonly IShippingService _shippingService;
		private readonly IAddressService _addressService;

		public OurStoresMenuViewComponent(IShippingService shippingService,
											IAddressService addressService)
		{
			this._shippingService = shippingService;
			this._addressService = addressService;
		}

		public IViewComponentResult Invoke(string active = "main")
		{
			ViewBag.Active = active;
			var warehouses = _shippingService.GetActiveWarehouses();
			WarehouseInfoModel model = new WarehouseInfoModel
			{
				Cities = warehouses.Select(x => x.City),
				Warehouses = warehouses
			};

			return View(model);
		}
	}
}
