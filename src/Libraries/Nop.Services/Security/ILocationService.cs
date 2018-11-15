using Nop.Core.Domain.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Security
{
    public interface ILocationService
    {
        FreeGeoIpModel GetLocation(string ip);
    }
}
