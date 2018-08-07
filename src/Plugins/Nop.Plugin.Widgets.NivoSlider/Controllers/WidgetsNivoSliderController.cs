using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.NivoSlider.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.NivoSlider.Controllers
{
    [Area(AreaNames.Admin)]
    public class WidgetsNivoSliderController : BasePluginController
    {
        private readonly IStoreContext _storeContext;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public WidgetsNivoSliderController(IStoreContext storeContext,
            IPermissionService permissionService, 
            IPictureService pictureService,
            ISettingService settingService,
            ICacheManager cacheManager,
            ILocalizationService localizationService)
        {
            this._storeContext = storeContext;
            this._permissionService = permissionService;
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._localizationService = localizationService;
        }

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var nivoSliderSettings = _settingService.LoadSetting<NivoSliderSettings>(storeScope);
            var model = new ConfigurationModel
            {
                Picture1Id = nivoSliderSettings.Picture1Id,
                Text1 = nivoSliderSettings.Text1,
                Link1 = nivoSliderSettings.Link1,
                AltText1 = nivoSliderSettings.AltText1,
                Picture2Id = nivoSliderSettings.Picture2Id,
                Text2 = nivoSliderSettings.Text2,
                Link2 = nivoSliderSettings.Link2,
                AltText2 = nivoSliderSettings.AltText2,
                Picture3Id = nivoSliderSettings.Picture3Id,
                Text3 = nivoSliderSettings.Text3,
                Link3 = nivoSliderSettings.Link3,
                AltText3 = nivoSliderSettings.AltText3,
                Picture4Id = nivoSliderSettings.Picture4Id,
                Text4 = nivoSliderSettings.Text4,
                Link4 = nivoSliderSettings.Link4,
                AltText4 = nivoSliderSettings.AltText4,
                Picture5Id = nivoSliderSettings.Picture5Id,
                Text5 = nivoSliderSettings.Text5,
                Link5 = nivoSliderSettings.Link5,
                AltText5 = nivoSliderSettings.AltText5,
                Picture6Id = nivoSliderSettings.Picture6Id,
                Text6 = nivoSliderSettings.Text6,
                Link6 = nivoSliderSettings.Link6,
                Picture7Id = nivoSliderSettings.Picture7Id,
                Text7 = nivoSliderSettings.Text7,
                Link7 = nivoSliderSettings.Link7,
                Picture8Id = nivoSliderSettings.Picture8Id,
                Text8 = nivoSliderSettings.Text8,
                Link8 = nivoSliderSettings.Link8,
                Picture9Id = nivoSliderSettings.Picture9Id,
                Text9 = nivoSliderSettings.Text9,
                Link9 = nivoSliderSettings.Link9,
                Picture10Id = nivoSliderSettings.Picture10Id,
                Text10 = nivoSliderSettings.Text10,
                Link10 = nivoSliderSettings.Link10,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.Picture1Id_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Picture1Id, storeScope);
                model.Text1_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Text1, storeScope);
                model.Link1_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Link1, storeScope);
                model.AltText1_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.AltText1, storeScope);
                model.Picture2Id_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Picture2Id, storeScope);
                model.Text2_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Text2, storeScope);
                model.Link2_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Link2, storeScope);
                model.AltText2_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.AltText2, storeScope);
                model.Picture3Id_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Picture3Id, storeScope);
                model.Text3_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Text3, storeScope);
                model.Link3_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Link3, storeScope);
                model.AltText3_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.AltText3, storeScope);
                model.Picture4Id_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Picture4Id, storeScope);
                model.Text4_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Text4, storeScope);
                model.Link4_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Link4, storeScope);
                model.AltText4_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.AltText4, storeScope);
                model.Picture5Id_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Picture5Id, storeScope);
                model.Text5_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Text5, storeScope);
                model.Link5_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Link5, storeScope);
                model.AltText5_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.AltText5, storeScope);
                model.Picture6Id_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Picture6Id, storeScope);
                model.Text6_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Text6, storeScope);
                model.Link6_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Link6, storeScope);
                model.Picture7Id_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Picture7Id, storeScope);
                model.Text7_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Text7, storeScope);
                model.Link7_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Link7, storeScope);
                model.Picture8Id_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Picture8Id, storeScope);
                model.Text8_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Text8, storeScope);
                model.Link8_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Link8, storeScope);
                model.Picture9Id_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Picture9Id, storeScope);
                model.Text9_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Text9, storeScope);
                model.Link9_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Link9, storeScope);
                model.Picture10Id_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Picture10Id, storeScope);
                model.Text10_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Text10, storeScope);
                model.Link10_OverrideForStore = _settingService.SettingExists(nivoSliderSettings, x => x.Link10, storeScope);
            }

            return View("~/Plugins/Widgets.NivoSlider/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var nivoSliderSettings = _settingService.LoadSetting<NivoSliderSettings>(storeScope);

            //get previous picture identifiers
            var previousPictureIds = new[] 
            {
                nivoSliderSettings.Picture1Id,
                nivoSliderSettings.Picture2Id,
                nivoSliderSettings.Picture3Id,
                nivoSliderSettings.Picture4Id,
                nivoSliderSettings.Picture5Id,
                nivoSliderSettings.Picture6Id,
                nivoSliderSettings.Picture7Id,
                nivoSliderSettings.Picture8Id,
                nivoSliderSettings.Picture9Id,
                nivoSliderSettings.Picture10Id
            };

            nivoSliderSettings.Picture1Id = model.Picture1Id;
            nivoSliderSettings.Text1 = model.Text1;
            nivoSliderSettings.Link1 = model.Link1;
            nivoSliderSettings.AltText1 = model.AltText1;
            nivoSliderSettings.Picture2Id = model.Picture2Id;
            nivoSliderSettings.Text2 = model.Text2;
            nivoSliderSettings.Link2 = model.Link2;
            nivoSliderSettings.AltText2 = model.AltText2;
            nivoSliderSettings.Picture3Id = model.Picture3Id;
            nivoSliderSettings.Text3 = model.Text3;
            nivoSliderSettings.Link3 = model.Link3;
            nivoSliderSettings.AltText3 = model.AltText3;
            nivoSliderSettings.Picture4Id = model.Picture4Id;
            nivoSliderSettings.Text4 = model.Text4;
            nivoSliderSettings.Link4 = model.Link4;
            nivoSliderSettings.AltText4 = model.AltText4;
            nivoSliderSettings.Picture5Id = model.Picture5Id;
            nivoSliderSettings.Text5 = model.Text5;
            nivoSliderSettings.Link5 = model.Link5;
            nivoSliderSettings.AltText5 = model.AltText5;
            nivoSliderSettings.Picture6Id = model.Picture6Id;
            nivoSliderSettings.Text6 = model.Text6;
            nivoSliderSettings.Link6 = model.Link6;
            nivoSliderSettings.Picture7Id = model.Picture7Id;
            nivoSliderSettings.Text7 = model.Text7;
            nivoSliderSettings.Link7 = model.Link7;
            nivoSliderSettings.Picture8Id = model.Picture8Id;
            nivoSliderSettings.Text8 = model.Text8;
            nivoSliderSettings.Link8 = model.Link8;
            nivoSliderSettings.Picture9Id = model.Picture9Id;
            nivoSliderSettings.Text9 = model.Text9;
            nivoSliderSettings.Link9 = model.Link9;
            nivoSliderSettings.Picture10Id = model.Picture10Id;
            nivoSliderSettings.Text10 = model.Text10;
            nivoSliderSettings.Link10 = model.Link10;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Picture1Id, model.Picture1Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Text1, model.Text1_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Link1, model.Link1_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.AltText1, model.AltText1_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Picture2Id, model.Picture2Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Text2, model.Text2_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Link2, model.Link2_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.AltText2, model.AltText2_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Picture3Id, model.Picture3Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Text3, model.Text3_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Link3, model.Link3_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.AltText3, model.AltText3_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Picture4Id, model.Picture4Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Text4, model.Text4_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Link4, model.Link4_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.AltText4, model.AltText4_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Picture5Id, model.Picture5Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Text5, model.Text5_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Link5, model.Link5_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.AltText5, model.AltText5_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Picture6Id, model.Picture6Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Text6, model.Text6_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Link6, model.Link6_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Picture7Id, model.Picture7Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Text7, model.Text7_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Link7, model.Link7_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Picture8Id, model.Picture8Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Text8, model.Text8_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Link8, model.Link8_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Picture9Id, model.Picture9Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Text9, model.Text9_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Link9, model.Link9_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Picture10Id, model.Picture10Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Text10, model.Text10_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(nivoSliderSettings, x => x.Link10, model.Link10_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();
            
            //get current picture identifiers
            var currentPictureIds = new[]
            {
                nivoSliderSettings.Picture1Id,
                nivoSliderSettings.Picture2Id,
                nivoSliderSettings.Picture3Id,
                nivoSliderSettings.Picture4Id,
                nivoSliderSettings.Picture5Id
            };

            //delete an old picture (if deleted or updated)
            foreach (var pictureId in previousPictureIds.Except(currentPictureIds))
            { 
                var previousPicture = _pictureService.GetPictureById(pictureId);
                if (previousPicture != null)
                    _pictureService.DeletePicture(previousPicture);
            }

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }
    }
}