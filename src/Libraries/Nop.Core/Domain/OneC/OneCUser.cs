using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Common;

namespace Nop.Core.Domain.OneC
{
    public class OneCUser
    {
        public int Id { get; set; }
        public int IdOneC { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Apartament { get; set; }
        public string NumberDiscountCard { get; set; }
        public int Percent { get; set; }
        public int SumActiveBonus { get; set; }
        public int SumBonus { get; set; }
        public int TotalSpent { get; set; }
    }
}