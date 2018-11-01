using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Shipping.NewPost.Domain
{
    public class NewPostCost
    {
        public int AssessedCost { get; set; }
        public int Cost { get; set; }
        public NewPostTZoneInfo TZoneInfo { get; set; }
    }
}
