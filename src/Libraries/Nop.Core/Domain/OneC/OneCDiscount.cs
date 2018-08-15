using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Discounts;

namespace Nop.Core.Domain.OneC
{
    public class OneCDiscount
    {
        public string Name { get; set; }
        public string DiscountType { get; set; }
        public bool UsePercentage { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal? MaximumDiscountAmount { get; set; }
        public bool RequiresCouponCode { get; set; }
        public string CouponCode { get; set; }
        public DateTime? StartDateUtc { get; set; }
        public DateTime? EndDateUtc { get; set; }
        public bool IsCumulative { get; set; }
        public string DiscountLimitation { get; set; }
    }
}
