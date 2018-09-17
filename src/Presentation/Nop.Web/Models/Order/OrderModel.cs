using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Order
{
    public class OrderModel : BaseNopEntityModel
	{
		public string Name { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public string DeliveryType { get; set; }
		public string Address1 { get; set; }
		public string City { get; set; }
		public string Building { get; set; }
		public string Apartment { get; set; }
		public string PaymentType { get; set; }
		public bool PayByBonuses { get; set; }
	}
}
