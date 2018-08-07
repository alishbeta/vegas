using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.OneC
{
    public class OneCProductsImport : OneCAuth
    {
        public List<OneCProduct> Products { get; set; }
    }
}
