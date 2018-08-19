using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Common;

namespace Nop.Core.Domain.OneC
{
    public class OneCUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public IList<Address> Addresses { get; set; }
    }
}
