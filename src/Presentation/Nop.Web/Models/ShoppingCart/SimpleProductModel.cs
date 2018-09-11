using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.ShoppingCart
{
    public class SimpleProductModel : BaseNopModel
	{
		public string Name { get; set; }
		public string SeName { get; set; }
		public string ShortDescription { get; set; }
		public string FullDescription { get; set; }
		public int ProductId { get; set; }
		public string ImageUrl { get; set; }
		public string Title { get; set; }
		public string AlternateText { get; set; }
		public decimal Discount { get; set; }
		public string OldPrice { get; set; }
		public string NewPrice { get; set; }
	}
}
