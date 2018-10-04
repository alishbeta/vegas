using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.LiqPay
{
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //PDT
            routeBuilder.MapRoute("Plugin.Payments.PaymentLiqPay.PDTHandler", "Plugins/PaymentLiqPay/PDTHandler",
                 new { controller = "PaymentLiqPay", action = "PDTHandler" });

            //IPN
            routeBuilder.MapRoute("Plugin.Payments.PaymentLiqPay.IPNHandler", "Plugins/PaymentLiqPay/IPNHandler",
                 new { controller = "PaymentLiqPay", action = "IPNHandler" });

            //Cancel
            routeBuilder.MapRoute("Plugin.Payments.PaymentLiqPay.CancelOrder", "Plugins/PaymentLiqPay/CancelOrder",
                 new { controller = "PaymentLiqPay", action = "CancelOrder" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority
        {
            get { return -1; }
        }
    }
}
