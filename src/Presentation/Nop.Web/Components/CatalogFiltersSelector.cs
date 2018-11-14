using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Components
{
    public class CatalogFiltersSelectorViewComponent : NopViewComponent
    {
        private readonly ICatalogModelFactory _catalogModelFactory;
        private readonly IWebHelper _webHelper;

        public CatalogFiltersSelectorViewComponent(ICatalogModelFactory catalogModelFactory, 
               IWebHelper webHelper)
        {
            this._catalogModelFactory = catalogModelFactory;
            this._webHelper = webHelper;
        }

        public IViewComponentResult Invoke(CatalogPagingFilteringModel.SpecificationFilterModel specificationFilter)
        {
            FilterRangesModel model = new FilterRangesModel();
            if (!string.IsNullOrEmpty(_webHelper.QueryString<string>("price")))
            {
                model.Price = new FilterRangesModel.FilterRange()
                {
                    from = double.Parse(_webHelper.QueryString<string>("price").Split('-')[0]),
                    to = double.Parse(_webHelper.QueryString<string>("price").Split('-')[1])
                };
            }
            if (!string.IsNullOrEmpty(_webHelper.QueryString<string>("height")))
            {
                model.Height = new FilterRangesModel.FilterRange()
                {
                    from = double.Parse(_webHelper.QueryString<string>("height").Split('-')[0]),
                    to = double.Parse(_webHelper.QueryString<string>("height").Split('-')[1])
                };
            }
            if (!string.IsNullOrEmpty(_webHelper.QueryString<string>("length")))
            {
                model.Length = new FilterRangesModel.FilterRange()
                {
                    from = double.Parse(_webHelper.QueryString<string>("length").Split('-')[0]),
                    to = double.Parse(_webHelper.QueryString<string>("length").Split('-')[1])
                };
            }
            if (!string.IsNullOrEmpty(_webHelper.QueryString<string>("width")))
            {
                model.Width = new FilterRangesModel.FilterRange()
                {
                    from = double.Parse(_webHelper.QueryString<string>("width").Split('-')[0]),
                    to = double.Parse(_webHelper.QueryString<string>("width").Split('-')[1])
                };
            }
            if (!string.IsNullOrEmpty(_webHelper.QueryString<string>("sleeplength")))
            {
                model.SleepLength = new FilterRangesModel.FilterRange()
                {
                    from = double.Parse(_webHelper.QueryString<string>("sleeplength").Split('-')[0]),
                    to = double.Parse(_webHelper.QueryString<string>("sleeplength").Split('-')[1])
                };
            }
            if (!string.IsNullOrEmpty(_webHelper.QueryString<string>("sleepwidth")))
            {
                model.SleepWidth = new FilterRangesModel.FilterRange()
                {
                    from = double.Parse(_webHelper.QueryString<string>("sleepwidth").Split('-')[0]),
                    to = double.Parse(_webHelper.QueryString<string>("sleepwidth").Split('-')[1])
                };
            }
            model.SpecificationFilter = specificationFilter;
            return View(model);
        }
    }
}
