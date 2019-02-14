using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.ComplexDiscounts;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Framework.Extensions;
using Nop.Web.Areas.Admin.Views.ComplexDiscount;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ComplexDiscountController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;
        private readonly IDiscountService _discountService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ILocalizationService _localizationService;

        public ComplexDiscountController(IPermissionService permissionService,
            IDiscountService discountService,
            IManufacturerService manufacturerService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ILocalizationService localizationService)
        {
            this._permissionService = permissionService;
            this._discountService = discountService;
            this._manufacturerService = manufacturerService;
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._localizationService = localizationService;
        }

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //prepare model
            var model = new ComplexDiscountSearchModel();

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(ComplexDiscountSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedKendoGridJson();

            //prepare model
            var discounts = _discountService.GetAllComplexDiscounts(searchModel.SearchDiscountGroupName);
            var model = new ComplexDiscountListModel()
            {
                Data = discounts.PaginationByRequestModel(searchModel).Select(discount =>
                {
                    return discount.ToModel<ComplexDiscountModel>();
                }),
                Total = discounts.Count
            };
            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //prepare model
            var model = new ComplexDiscountModel();
            _baseAdminModelFactory.PrepareManufacturers(model.Manufacturers, true, _localizationService.GetResource("Admin.ComplexDiscount.Manufacturer.NotSelected"));
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(ComplexDiscountModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var discount = model.ToEntity<ComplexDiscount>();
                _discountService.InsertComplexDiscount(discount);
                
                SuccessNotification(_localizationService.GetResource("Admin.Promotions.ComplexDiscounts.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = discount.Id });
            }

            //prepare manufacturers
            _baseAdminModelFactory.PrepareManufacturers(model.Manufacturers, true, _localizationService.GetResource("Admin.ComplexDiscount.Manufacturer.NotSelected"));

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = _discountService.GetComplexDiscountById(id);
            if (discount == null)
                return RedirectToAction("List");

            //prepare model
            var model = discount.ToModel<ComplexDiscountModel>();
            _baseAdminModelFactory.PrepareManufacturers(model.Manufacturers, true, _localizationService.GetResource("Admin.ComplexDiscount.Manufacturer.NotSelected"));

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(ComplexDiscountModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = _discountService.GetComplexDiscountById(model.Id);
            if (discount == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                discount = model.ToEntity(discount);
                _discountService.UpdateComplexDiscount(discount);
                
                SuccessNotification(_localizationService.GetResource("Admin.Promotions.Discounts.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = discount.Id });
            }

            //prepare manufacturers
            _baseAdminModelFactory.PrepareManufacturers(model.Manufacturers, true, _localizationService.GetResource("Admin.ComplexDiscount.Manufacturer.NotSelected"));

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //try to get a discount with the specified id
            var discount = _discountService.GetComplexDiscountById(id);
            if (discount == null)
                return RedirectToAction("List");

            _discountService.DeleteComplexDiscount(discount);

            SuccessNotification(_localizationService.GetResource("Admin.Promotions.ComplexDiscounts.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(int[] selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            foreach (var id in selectedIds)
            {
                //try to get a discount with the specified id
                var discount = _discountService.GetComplexDiscountById(id);
                if (discount == null)
                    continue;

                _discountService.DeleteComplexDiscount(discount);
            }

            SuccessNotification(_localizationService.GetResource("Admin.Promotions.ComplexDiscounts.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult CopyComplexDiscount(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            try
            {
                var copyModel = _discountService.GetComplexDiscountById(id);
                var newModel = new ComplexDiscount()
                {
                    InCollection = copyModel.InCollection,
                    InManufacturerId = copyModel.InManufacturerId,
                    GroupName = copyModel.GroupName,
                    InModel = copyModel.InModel,
                    ComCollection = copyModel.ComCollection,
                    ComManufacturerId = copyModel.ComManufacturerId,
                    ComModel = copyModel.ComModel,
                    ComType = copyModel.ComType,
                    DiscountPercent = copyModel.DiscountPercent,
                    InType = copyModel.InType,
                    Name = string.Format("{0} ({1})", copyModel.Name, _localizationService.GetResource("Admin.ComplexDiscount.Copy"))
                };

                _discountService.InsertComplexDiscount(newModel);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.ComplexDiscount.Copied"));

                return RedirectToAction("Edit", new { id = newModel.Id });
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = id });
            }
        }
    }
}
