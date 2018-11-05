﻿using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Shipping
{
    public class WarehouseInfoModel
    {
		public IList<Warehouse> Warehouses { get; set; }
		public List<string> Cities { get; set; }
		public List<Address> Addresses { get; set; }
	}
}