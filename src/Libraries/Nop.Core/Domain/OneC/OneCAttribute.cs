using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.OneC
{
    public class OneCAttribute
    {
        public string Name { get; set; }
        public int UiType { get; set; }
        public List<OneCAttributeValue> AttributeValues { get; set; }
    }
}
