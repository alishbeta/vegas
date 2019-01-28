using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.OneC
{
    public class OneCOrdersImport : OneCAuth
    {
        public List<OneCOrder> Orders { get; set; }
    }
}
