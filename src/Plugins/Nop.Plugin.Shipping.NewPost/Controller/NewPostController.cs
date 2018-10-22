using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Shipping.NewPost.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Shipping.NewPost.Controller
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class NewPostController : BaseController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly NewPostSettings _newPostSettings;

        #endregion

        #region Ctor

        public NewPostController(ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            NewPostSettings newPostSettings)
        {
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._settingService = settingService;
            this._newPostSettings = newPostSettings;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new NewPostModel()
            {
                ApiKey = _newPostSettings.ApiKey,
                Url = _newPostSettings.Url
            };

            return View("~/Plugins/Shipping.NewPost/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult Configure(NewPostModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            _newPostSettings.ApiKey = model.ApiKey;
            _newPostSettings.Url = model.Url;
            _settingService.SaveSetting(_newPostSettings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }

        #endregion
    }
}
