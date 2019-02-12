using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.ComplexDiscounts
{
    public class ComplexDiscountModel : BaseNopEntityModel
    {

        public string Name { get; set; }
        public decimal DiscountPercent { get; set; }
        public string InType { get; set; }
        public int InManufacturerId { get; set; }
        public string InCollection { get; set; }
        public string InModel { get; set; }
        public string ComType { get; set; }
        public int ComManufacturerId { get; set; }
        public string ComCollection { get; set; }
        public string ComModel { get; set; }
    }
}
