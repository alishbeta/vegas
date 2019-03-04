using System;
using System.Collections.Generic;
using Nop.Core.Domain.Shipping;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Common;

namespace Nop.Web.Models.Common
{
    public class CityMapModel
    {
		public IEnumerable<ViewWarehouseModel> Warehouses { get; set; }
		public string Name { get; set; }
	}
}
