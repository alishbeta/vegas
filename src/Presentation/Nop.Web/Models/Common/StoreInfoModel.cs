﻿using Nop.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Common
{
    public class StoreInfoModel
    {
		public Address Address { get; set; }
		public List<Address> OtherStores { get; set; }
	}
}
