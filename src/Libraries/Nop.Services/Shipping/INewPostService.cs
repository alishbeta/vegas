using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Shipping
{
    public interface INewPostService
    {
        string GetCityId(string cityName);
        int GetCost(string cityIdFrom, string cityIdTo, string weight, string assessedCost);
    }
}
