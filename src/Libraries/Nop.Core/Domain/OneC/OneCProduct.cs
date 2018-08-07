using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.OneC
{
    public class OneCProduct
    {
        public string Name { get; set; }
        public List<OneCWarehouse> Warehouses { get; set; }
        public string Manufacturer { get; set; }
        public decimal Price { get; set; }
        public string Sku { get; set; }
        public string Status { get; set; }
        public List<OneCAttribute> Attributes { get; set; }
    }
}
