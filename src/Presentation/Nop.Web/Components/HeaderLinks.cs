using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class HeaderLinksViewComponent : NopViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly ILocationService _locationService;

        public HeaderLinksViewComponent(ICommonModelFactory commonModelFactory,
            ILocationService locationService)
        {
            this._commonModelFactory = commonModelFactory;
            this._locationService = locationService;
        }

        public IViewComponentResult Invoke()
        {
            var model = _commonModelFactory.PrepareHeaderLinksModel();
            return View(model);
        }
    }
}
