using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Catalog
{
    public class ProductWarehouse
    {
        public string Name { get; set; }
        public int WarehouseId { get; set; }
        public int StockQuantity { get; set; }
    }
}
