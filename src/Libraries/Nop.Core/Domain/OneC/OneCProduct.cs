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
        public string Price { get; set; }
        public string Sku { get; set; }
        public string Status { get; set; }
        public string Weight { get; set; }
        public string Length { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Model { get; set; }
        public string SleepWeight { get; set; }
        public string SleepLength { get; set; }
        public string SleepWidth { get; set; }
        public string SleepHeight { get; set; }
        public string DiscountPrice { get; set; }
        public string DiscountRate { get; set; }
        public string Collection { get; set; }
        public string TypeProduct { get; set; }
        public List<OneCAttribute> Attributes { get; set; }
    }
}
