using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Shipping.NewPost.Domain
{
    public class NewPostAddress
    {
        public string Present { get; set; }
        public long Warehouses { get; set; }
        public string MainDescription { get; set; }
        public string Area { get; set; }
        public string Region { get; set; }
        public string SettlementTypeCode { get; set; }
        public string Ref { get; set; }
        public string DeliveryCity { get; set; }
        public bool StreetsAvailability { get; set; }
        public string ParentRegionTypes { get; set; }
        public string ParentRegionCode { get; set; }
        public string RegionTypes { get; set; }
        public string RegionTypesCode { get; set; }
    }
}
