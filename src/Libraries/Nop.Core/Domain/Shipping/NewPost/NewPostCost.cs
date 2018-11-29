using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Shipping.NewPost;

namespace Nop.Core.Domain.Shipping.NewPost
{
    public class NewPostCost
    {
        public int AssessedCost { get; set; }
        public int Cost { get; set; }
        public NewPostTZoneInfo TZoneInfo { get; set; }
    }
}
