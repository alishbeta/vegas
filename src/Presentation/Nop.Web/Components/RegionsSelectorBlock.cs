using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Framework.Components;
using System.Linq;

namespace Nop.Web.Components
{
    public class RegionsSelectorBlockViewComponent : NopViewComponent
    {
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        private readonly ILanguageModelFactory _languageModelFactory;

        public RegionsSelectorBlockViewComponent(ILanguageService languageService,
                                                 IWorkContext workContext,
                                                 ILanguageModelFactory languageModelFactory)
        {
            this._languageService = languageService;
            this._workContext = workContext;
            this._languageModelFactory = languageModelFactory;
        }

        public IViewComponentResult Invoke(string selectedCity)
        {
            var language = _languageService.GetLanguageById(_workContext.WorkingLanguage.Id, false);

            //prepare model
            var resources = _languageModelFactory.PrepareLocaleResourceModel(new LocaleResourceSearchModel()
            {
                SearchResourceName = "regions."
            }, language);

            if (!string.IsNullOrEmpty(selectedCity))
            {
                ViewBag.SelectedCity = _languageModelFactory.PrepareLocaleResourceModel(new LocaleResourceSearchModel()
                {
                    SearchResourceValue = selectedCity.ToLower(),
                    SearchResourceName = "regions."
                }, language).Data.Where(x => x.Id >= 27476 && x.Id <= 27571).FirstOrDefault()?.Name;
            }
            else
            {
                ViewBag.SelectedCity = null;
            }

            if (_workContext.WorkingLanguage.Id == 4)
            {
                var model = resources.Data.Where(x => x.Id >= 27524 && x.Id <= 27571).OrderBy(x => x.Value);
                return View(model);
            }
            else
            {
                var model = resources.Data.Where(x => x.Id >= 27476 && x.Id <= 27523).OrderBy(x => x.Value);
                return View(model);
            }
        }
    }
}
