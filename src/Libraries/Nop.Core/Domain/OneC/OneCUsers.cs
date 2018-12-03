using System.Collections.Generic;

namespace Nop.Core.Domain.OneC
{
    public class OneCUsers : OneCAuth
    {
        public IList<OneCUser> Users { get; set; }
    }
}