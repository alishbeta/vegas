using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Common
{
    public class ViewWarehouseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WorkTime { get; set; }
        public string WarehouseDescription { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string StreetAddress { get; set; }
        public List<WarehousePicture> Pictures { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public class WarehousePicture
        {
            public string PictureUrl { get; set; }
        }
    }
}
