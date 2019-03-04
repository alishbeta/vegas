using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Common
{
    public class StoreInfoModel
    {
		public Warehouse Warehouse { get; set; }
		public IList<Warehouse> Warehouses { get; set; }
		public IList<ViewWarehouseModel> WarehouseViewModels { get; set; }
		public IList<Warehouse> OtherStores { get; set; }
	}
}
