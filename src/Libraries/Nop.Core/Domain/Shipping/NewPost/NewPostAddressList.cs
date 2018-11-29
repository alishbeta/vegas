using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Shipping.NewPost
{
    public class NewPostAddressList
    {
        public long TotalCount { get; set; }
        public List<NewPostAddress> Addresses { get; set; }
    }
}
