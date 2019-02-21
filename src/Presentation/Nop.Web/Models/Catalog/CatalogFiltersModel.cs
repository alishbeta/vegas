using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Catalog
{
    public class CatalogFiltersModel
    {
        public CatalogFiltersModel()
        {
            this.Groups = new List<CategoryGroup>();
        }
		public string orderby { get; set; }
        public IEnumerable<CategoryGroup> Groups { get; set; }
	}
}
