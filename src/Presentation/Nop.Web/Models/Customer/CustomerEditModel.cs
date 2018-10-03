using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Customer
{
    public class CustomerEditModel : BaseNopModel
    {
		public string Name { get; set; }
		public string Phone { get; set; }
		public string Address { get; set; }
		public string Building { get; set; }
		public string Flat { get; set; }
		public string Password { get; set; }
	}
}
