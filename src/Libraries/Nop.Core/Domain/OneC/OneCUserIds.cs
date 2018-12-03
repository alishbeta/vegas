using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.OneC
{
    public class OneCUserIds : OneCAuth
    {
        public IList<int> Ids { get; set; }
    }
}
