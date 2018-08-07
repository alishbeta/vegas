using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{
    public class ProductWarehouseStatus : BaseEntity
    {
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int StatusId { get; set; }
    }
}
