using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.LiqPay.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.LiqPay.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.LiqPay.Fields.PublicKey")]
        public string PublicKey { get; set; }
        public bool PublicKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.LiqPay.Fields.PrivateKey")]
        public string PrivateKey { get; set; }
        public bool PrivateKey_OverrideForStore { get; set; }

    }
}