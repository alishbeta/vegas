using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.OneC
{
    public class OneCOrderProductInfo
    {
        public string ProduxtSku { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
    }
}
