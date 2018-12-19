using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Shipping
{
    public partial class WarehousePictures : BaseEntity
    {
        public int WarehouseId { get; set; }
        public int PictureId { get; set; }
    }
}
