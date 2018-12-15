using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using System.Collections.Generic;
using System.Linq;

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

        public IViewComponentResult Invoke(CatalogPagingFilteringModel.SpecificationFilterModel specificationFilter, IEnumerable<Product> products)
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
            
            model.Price.max = (double)(products.OrderByDescending(x => x.Price).FirstOrDefault()?.Price ?? 0);
            model.Height.max = (double)(products.OrderByDescending(x => x.Height).FirstOrDefault()?.Height ?? 0);
            model.Length.max = (double)(products.OrderByDescending(x => x.Length).FirstOrDefault()?.Length ?? 0);
            model.Width.max = (double)(products.OrderByDescending(x => x.Width).FirstOrDefault()?.Width ?? 0);
            model.SleepLength.max = (products.OrderByDescending(x => x.SleepLength).FirstOrDefault()?.SleepLength ?? 0);
            model.SleepWidth.max = (products.OrderByDescending(x => x.SleepWidth).FirstOrDefault()?.SleepWidth ?? 0);
            
            model.SpecificationFilter = specificationFilter;
            return View(model);
        }
    }
}
