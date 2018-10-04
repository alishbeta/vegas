using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.LiqPay
{
    /// <summary>
    /// Represents settings of the PayPal Standard payment plugin
    /// </summary>
    public class LiqPayPaymentSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox (testing environment)
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets Public Key
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets Private Key
        /// </summary>
        public string PrivateKey { get; set; }
    }
}
