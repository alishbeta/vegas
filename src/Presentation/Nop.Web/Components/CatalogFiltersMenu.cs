using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Components
{
    public class CatalogFiltersMenuViewComponent : NopViewComponent
	{
		private readonly IWebHelper _webHelper;
												
		public CatalogFiltersMenuViewComponent(IWebHelper webHelper)
		{
			this._webHelper = webHelper;
		}

		public IViewComponentResult Invoke()
		{
			CatalogFiltersModel model = new CatalogFiltersModel();
			model.orderby = _webHelper.QueryString<string>("orderby") ?? "1";
			return View(model);
		}
	}
}
