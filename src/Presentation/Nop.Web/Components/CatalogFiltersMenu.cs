using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Seo;
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
        private readonly ICategoryService _categoryService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
												
		public CatalogFiltersMenuViewComponent(IWebHelper webHelper,
            ICategoryService categoryService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
		{
			this._webHelper = webHelper;
            this._categoryService = categoryService;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
		}

		public IViewComponentResult Invoke(int currentCategory)
		{
			CatalogFiltersModel model = new CatalogFiltersModel();
			model.orderby = _webHelper.QueryString<string>("orderby") ?? "1";
            var category = _categoryService.GetCategoryById(currentCategory);
            if (category.ParentCategoryId != 0)
            {
                currentCategory = category.ParentCategoryId;
            }
            var groups = _categoryService.GetChildGroups(currentCategory);
            model.Groups = groups.Select(x => new CategoryGroup()
            {
                Deleted = x.Deleted,
                DisplayOrder = x.DisplayOrder,
                IsGroup = x.IsGroup,
                Name = x.Name,
                ParentCategoryId = x.ParentCategoryId,
                Published = x.Published,
                SeName = _urlRecordService.GetSeName(x, _workContext.WorkingLanguage.Id, false, false),
                ChildGroups = _categoryService.GetChildGroups(x.Id).Select(u => new CategoryGroup()
                {
                    Deleted = u.Deleted,
                    DisplayOrder = u.DisplayOrder,
                    IsGroup = u.IsGroup,
                    Name = u.Name,
                    ParentCategoryId = u.ParentCategoryId,
                    Published = u.Published,
                    SeName = _urlRecordService.GetSeName(u, _workContext.WorkingLanguage.Id, false, false),
                    ChildGroups = null
                })
            });
			return View(model);
		}
	}
}
