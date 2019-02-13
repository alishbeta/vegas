using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Validators.ComplexDiscount;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.ComplexDiscounts
{
    [Validator(typeof(ComplexDiscountValidator))]
    public class ComplexDiscountModel : BaseNopEntityModel
    {
        public ComplexDiscountModel()
        {
            Manufacturers = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Promotions.ComplexDiscounts.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Promotions.ComplexDiscounts.Fields.DiscountPercent")]
        public decimal DiscountPercent { get; set; }

        [NopResourceDisplayName("Admin.Promotions.ComplexDiscounts.Fields.InType")]
        public string InType { get; set; }

        [NopResourceDisplayName("Admin.Promotions.ComplexDiscounts.Fields.InManufacturerId")]
        public int InManufacturerId { get; set; }

        [NopResourceDisplayName("Admin.Promotions.ComplexDiscounts.Fields.InCollection")]
        public string InCollection { get; set; }

        [NopResourceDisplayName("Admin.Promotions.ComplexDiscounts.Fields.InModel")]
        public string InModel { get; set; }

        [NopResourceDisplayName("Admin.Promotions.ComplexDiscounts.Fields.ComType")]
        public string ComType { get; set; }

        [NopResourceDisplayName("Admin.Promotions.ComplexDiscounts.Fields.ComManufacturerId")]
        public int ComManufacturerId { get; set; }

        [NopResourceDisplayName("Admin.Promotions.ComplexDiscounts.Fields.ComCollection")]
        public string ComCollection { get; set; }

        [NopResourceDisplayName("Admin.Promotions.ComplexDiscounts.Fields.ComModel")]
        public string ComModel { get; set; }

        public IList<SelectListItem> Manufacturers { get; set; }
    }
}
