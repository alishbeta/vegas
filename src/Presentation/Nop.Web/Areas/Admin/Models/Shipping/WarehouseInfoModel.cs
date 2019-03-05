using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Shipping
{
    public class WarehouseInfoModel
    {
		public IList<ViewWarehouseModel> Warehouses { get; set; }
		public IEnumerable<LocalizedCityModel> Cities { get; set; }
	}
    public partial class LocalizedCityModel
    {
        public string City { get; set; }
        public string LocalizedCity { get; set; }
    }
}
