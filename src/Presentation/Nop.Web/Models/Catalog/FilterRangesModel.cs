using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Catalog
{
    public class FilterRangesModel
    {
        public FilterRangesModel()
        {
            Height = new FilterRange();
            Width = new FilterRange();
            Length = new FilterRange();
            SleepLength = new FilterRange();
            SleepWidth = new FilterRange();
            Price = new FilterRange();
        }
        public int CategoryId { get; set; }
        public FilterRange Height { get; set; }
        public FilterRange Width { get; set; }
        public FilterRange Length { get; set; }
        public FilterRange SleepLength { get; set; }
        public FilterRange SleepWidth { get; set; }
        public FilterRange Price { get; set; }
        public class FilterRange
        {
            public double from { get; set; }
            public double to { get; set; }
            public double min { get; set; }
            public double max { get; set; }
        }
        public CatalogPagingFilteringModel.SpecificationFilterModel SpecificationFilter { get; set; }
    }
}
