using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Catalog
{
    public class CategoryGroup
    {
        public CategoryGroup()
        {
            this.ChildGroups = new List<CategoryGroup>();
        }
        public bool IsGroup { get; set; }
        public bool Published { get; set; }
        public bool Deleted { get; set; }
        public int ParentCategoryId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public string SeName { get; set; }
        public IEnumerable<CategoryGroup> ChildGroups { get; set; }
    }
}
