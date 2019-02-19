using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.ComplexDiscounts
{
    public class ComplexDiscountSearchModel : BaseSearchModel
    {        
        [NopResourceDisplayName("Admin.Promotions.ComplexDiscounts.List.SearchDiscountName")]
        public string SearchDiscountGroupName { get; set; }
    }
}
