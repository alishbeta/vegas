using Newtonsoft.Json;
using Nop.Plugin.Payments.LiqPay.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Nop.Plugin.Payments.LiqPay
{
	public class LiqPaySignatureGenerator
	{
		private MD5 _md5;
		private SHA1 _sha1;
		private Encoding _encoding;
		public LiqPaySignatureGenerator()
		{
			_md5 = MD5.Create();
			_sha1 = SHA1.Create();
			_encoding = Encoding.UTF8;
		}


		public void GenerateSignature(PaymentLiqpayVM/*Our view model*/ payment)
		{
			string result = JsonConvert.SerializeObject(payment);
			string data = Convert.ToBase64String(Encoding.UTF8.GetBytes(result));
			//string signature = Convert.ToBase64String(Encoding.UTF8.GetBytes(StringToSHA1(payment.private_key + data + payment.private_key)));
			string signature = Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(payment.private_key + data + payment.private_key)));
			payment.data = data;
			payment.signature = signature;
			//return payment;
		}

		public string GenerateSignature(string data, string password)
		{
			string signature = Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(password + data + password)));

			return signature;
		}



		private string StringToSHA1(string source)
		{
			var sha1 = SHA1.Create();
			var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(source));
			var sb = new StringBuilder();
			foreach (byte temp in hash)
			{
				sb.AppendFormat("{0:x2}", temp);
			}
			return sb.ToString();
		}
	}
}
