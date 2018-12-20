using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Common
{
    public class ViewWarehouseModel
    {
        public string Name { get; set; }
        public int AddressId { get; set; }
        public string WorkTime { get; set; }
        public List<WarehousePicture> Pictures { get; set; }

        public class WarehousePicture
        {
            public string PictureUrl { get; set; }
        }
    }
}
