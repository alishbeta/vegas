using Microsoft.AspNetCore.Mvc;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Common;
using NUglify.Helpers;
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
        private readonly ILocalizationService _localizationService;

		public OurStoresMenuViewComponent(IShippingService shippingService,
											IAddressService addressService,
                                            ILocalizationService localizationService)
		{
			this._shippingService = shippingService;
			this._addressService = addressService;
            this._localizationService = localizationService;
		}

		public IViewComponentResult Invoke(string active = "main")
		{
			//ViewBag.Active = active;
			var warehouses = _shippingService.GetActiveWarehouses();
            var activeWarehouse = warehouses.FirstOrDefault(x => x.City?.ToLower() == active?.ToLower() || _localizationService.GetLocalized(x, u => u.City)?.ToLower() == active?.ToLower());
            if (activeWarehouse != null)
            {
                ViewBag.Active = _localizationService.GetLocalized(activeWarehouse, c => c.City);
            }
            else
            {
                ViewBag.Active = active;
            }
            var viewWarehouseModelList = new List<ViewWarehouseModel>();
            warehouses.ToList().ForEach(x =>
            {
                viewWarehouseModelList.Add(new ViewWarehouseModel()
                {
                    Id = x.Id,
                    Name = _localizationService.GetLocalized(x, u => u.Name),
                    WorkTime = _localizationService.GetLocalized(x, u => u.WorkTime),
                    WarehouseDescription = _localizationService.GetLocalized(x, u => u.WarehouseDescription),
                    StreetAddress = _localizationService.GetLocalized(x, u => u.StreetAddress),
                    Phone = _localizationService.GetLocalized(x, u => u.Phone),
                    City = _localizationService.GetLocalized(x, u => u.City)
                });
            });
            WarehouseInfoModel model = new WarehouseInfoModel
			{
				Cities = warehouses
                .Where(x => x.City != null)
                .Select(x => new LocalizedCityModel()
                {
                    City = x.City,
                    LocalizedCity = _localizationService.GetLocalized(x, u => u.City)
                }).DistinctBy(x => x.City),
				Warehouses = viewWarehouseModelList
            };

			return View(model);
		}
	}
}
