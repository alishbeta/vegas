using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.OneC
{
    public class OneCOrder
    {
        public int OrderNumber { get; set; }
        public string DeliveryMethod { get; set; }
        public string DeliveryAddress { get; set; }
        public string BillingMethod { get; set; }
        public string CustomerContact { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public IEnumerable<OneCOrderProductInfo> Products { get; set; }
    }
}
