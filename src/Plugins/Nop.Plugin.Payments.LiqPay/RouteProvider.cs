using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
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
            routeBuilder.MapLocalizedRoute("Plugin.Payments.PaymentLiqPay.Checkout", "/checkout/liqpay",
                 new { controller = "PaymentLiqPay", action = "CreatePayment" });

            //IPN
            routeBuilder.MapRoute("Plugin.Payments.PaymentLiqPay.Callback", "/liqpay/callback",
                 new { controller = "PaymentLiqPay", action = "ServerUrl" });

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
