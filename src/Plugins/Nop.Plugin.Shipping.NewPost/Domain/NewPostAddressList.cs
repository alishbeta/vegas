using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Shipping.NewPost.Domain
{
    public class NewPostAddressList
    {
        public long TotalCount { get; set; }
        public List<NewPostAddress> Addresses { get; set; }
    }
}
