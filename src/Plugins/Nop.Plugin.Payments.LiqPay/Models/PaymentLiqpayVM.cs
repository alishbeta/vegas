using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.LiqPay.Models
{
	public class PaymentLiqpayVM
	{
		public string version { get; set; }
		public string public_key { get; set; }
		public string private_key { get; set; }

		public string action { get; set; }
		public string amount { get; set; }
		public string currency { get; set; }
		public string description { get; set; }
		public string order_id { get; set; }
		public string recurringbytoken { get; set; }
		public string server_url { get; set; }
		public string result_url { get; set; }
		public string pay_way { get; set; }
		public string language { get; set; }
		public string sandbox { get; set; }
		public string data { get; set; }
		public string signature { get; set; }
		public string status { get; set; }
	}

	public class PaymentLiqpayResponse : PaymentLiqpayVM
	{
		public string err_code { get; set; }
		public string err_description { get; set; }
	}
}
