using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Catalog
{
    public class FilterValuesModel
    {
		public string FilterName { get; set; }
		public List<string> FilterValues { get; set; }
	}
}
