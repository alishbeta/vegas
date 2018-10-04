using Nop.Core.Domain.Payments;

namespace Nop.Plugin.Payments.LiqPay
{
    public class LiqPayHelper
    {
        public static PaymentStatus GetPaymentStatus(string paymentStatus, string pendingReason)
        {
            //var result = PaymentStatus.Pending;

            //if (paymentStatus == null)
            //    paymentStatus = string.Empty;

            //if (pendingReason == null)
            //    pendingReason = string.Empty;

            //switch (paymentStatus.ToLowerInvariant())
            //{
            //    case "pending":
            //        switch (pendingReason.ToLowerInvariant())
            //        {
            //            case "authorization":
            //                result = PaymentStatus.Authorized;
            //                break;
            //            default:
            //                result = PaymentStatus.Pending;
            //                break;
            //        }
            //        break;
            //    case "processed":
            //    case "completed":
            //    case "canceled_reversal":
            //        result = PaymentStatus.Paid;
            //        break;
            //    case "denied":
            //    case "expired":
            //    case "failed":
            //    case "voided":
            //        result = PaymentStatus.Voided;
            //        break;
            //    case "refunded":
            //    case "reversed":
            //        result = PaymentStatus.Refunded;
            //        break;
            //    default:
            //        break;
            //}
            return PaymentStatus.Voided;
        }
    }
}