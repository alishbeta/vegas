using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.OneC
{
    public class OneCPaging : OneCAuth
    {
        public int Take { get; set; }
        public int Skip { get; set; }
    }
}
