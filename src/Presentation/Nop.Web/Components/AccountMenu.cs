using Microsoft.AspNetCore.Mvc;
using Nop.Services.Common;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Customer;
using System.Collections.Generic;  

namespace Nop.Web.Components
{
    public class AccountMenuViewComponent : NopViewComponent
	{
		private readonly IShippingService _shippingService;
		private readonly IAddressService _addressService;

		public AccountMenuViewComponent(IShippingService shippingService,
											IAddressService addressService)
		{
			this._shippingService = shippingService;
			this._addressService = addressService;
		}

		public IViewComponentResult Invoke(string active = "/customer/info")
		{
			MenuItem model = new MenuItem() { href = active };
			return View(model);
		}
	}
}
