using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Discounts
{
    public partial class ComplexDiscount : BaseEntity
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
