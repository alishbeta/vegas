using Nop.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Shipping
{
    public partial interface INewPostHelper : IPlugin
    {
        string GetCityId(string cityName);
        int GetCost(string cityIdFrom, string cityIdTo, string weight, string assessedCost);
    }
}
