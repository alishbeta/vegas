using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.LiqPay.Components
{
    [ViewComponent(Name = "PaymentLiqPay")]
    public class PaymentLiqPayViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Payments.LiqPay/Views/PaymentInfo.cshtml");
        }
    }
}
