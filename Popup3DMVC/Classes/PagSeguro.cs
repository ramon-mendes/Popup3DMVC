using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Popup3DMVC.Models;

namespace Popup3DMVC.Classes
{
    public static class PagSeguro
    {
		public static string GenerateToken(PedidoModel pedido)
		{
			try
			{
				Dictionary<string, string> postData = new Dictionary<string, string>();
				postData["email"] = "ramon@misoftware.com.br";
				postData["token"] = "C915D608A54246769EFCE40F347CDD41";
				postData["currency"] = "BRL";
				postData["itemId1"] = "0001";
				postData["itemDescription1"] = pedido.Upload.filename;
				postData["itemAmount1"] = pedido.valor_total.ToString("F2");
				postData["itemQuantity1"] = "1";
				postData["itemWeight1"] = "1000";
				postData["reference"] = "REF1234";
				postData["senderName"] = pedido.Checkout.Nome;
				//postData["senderAreaCode"] = "99";
				//postData["senderPhone"] = "99999999";
				postData["senderEmail"] = pedido.Checkout.Email;
				postData["shippingType"] = "1";
				postData["shippingAddressStreet"] = pedido.Checkout.Rua;
				postData["shippingAddressNumber"] = pedido.Checkout.Número;
				postData["shippingAddressComplement"] = pedido.Checkout.Complemento;
				postData["shippingAddressDistrict"] = pedido.Checkout.Bairro;
				postData["shippingAddressPostalCode"] = pedido.Checkout.CEP;
				postData["shippingAddressCity"] = pedido.Checkout.Cidade;
				postData["shippingAddressState"] = pedido.Checkout.UF;
				postData["shippingAddressCountry"] = "ATA";

				using(var httpClient = new HttpClient())
				{
					using(var content = new FormUrlEncodedContent(postData))
					{
						content.Headers.Clear();
						content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

						HttpResponseMessage response = httpClient.PostAsync("https://ws.sandbox.pagseguro.uol.com.br/v2/checkout/", content).Result;
						var xml = response.Content.ReadAsStringAsync().Result;
						response.EnsureSuccessStatusCode();
						var math = Regex.Match(xml, "<code>(.*?)</code>");
						return math.Groups[1].Value;
					}
				}
			}
			catch(Exception)
			{
			}
			return null;
		}
    }
}