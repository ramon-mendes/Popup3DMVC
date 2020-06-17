using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Device.Location;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Diagnostics;
using System.Threading;

namespace Popup3DMVC.Classes
{
	public static class Utils
	{
		public class EnderecoCEP
		{
			public string cep { get; set; }
			public string rua { get; set; }
			public string bairro { get; set; }
			public string cidade { get; set; }
			public string UF { get; set; }
		}

		public static EnderecoCEP BuscaCEP(string cep)
		{
			cep = cep.Replace("-", string.Empty);// remove o hífen
			Match regex = Regex.Match(cep, "^[0-9]{8}$");
			if(!regex.Success)
				return null;

			EnderecoCEP addr = null;
			using(HttpClient hc = new HttpClient())
			{
				var values = new Dictionary<string, string>
				{
					{ "relaxation", cep },
					{ "tipoCep", "ALL" },
					{ "semelhante", "N" }
				};

				var content = new FormUrlEncodedContent(values);
				var response = hc.PostAsync("http://www.buscacep.correios.com.br/sistemas/buscacep/resultadoBuscaCepEndereco.cfm", content).Result;
				var responseText = response.Content.ReadAsStringAsync().Result;

				MatchCollection matches = Regex.Matches(responseText, "<td.*?>(.*?)</td>");
				if(matches.Count == 2)
				{
					addr = new EnderecoCEP
					{
						cep = cep,
						cidade = matches[0].Groups[1].Value.Replace("&nbsp;", ""),
						UF = matches[1].Groups[1].Value.Replace("&nbsp;", ""),
					};
				}
				else if(matches.Count == 4)
				{
					string cidade_estado = matches[2].Groups[1].Value.Replace("&nbsp;", "");
					string[] split = cidade_estado.Split('/');
					string rua = matches[0].Groups[1].Value.Replace("&nbsp;", "");
					var rua_match = Regex.Matches(rua, "<a.*?>(.*?)<br><br>.*?</a>");
					if(rua_match.Count==1)
					{
						rua = rua_match[0].Groups[1].Value;
					}

					addr = new EnderecoCEP
					{
						cep = cep,
						rua = rua,
						bairro = matches[1].Groups[1].Value.Replace("&nbsp;", ""),
						cidade = split[0],
						UF = split[1]
					};
				}
			}
			return addr;
		}

		public static GeoCoordinate BuscaCoordEnd(EnderecoCEP end)
		{
			return BuscaCoordEnd(end.cidade, end.rua, end.bairro, "", end.UF);
		}

		public static GeoCoordinate BuscaCoordEnd(string cidade, string rua, string bairro, string numero, string estado)
		{
			string param = Uri.EscapeDataString($"{rua}, {numero}, {bairro}, {cidade}, {estado}");
			WebRequest request = WebRequest.Create("https://maps.google.com/maps/api/geocode/json?key=AIzaSyB2Lvl1LA5WQN7-_B4XZiGhnmFLeYtRrek&address=" + param);
			StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
			string responseText = reader.ReadToEnd();

			dynamic json = JsonConvert.DeserializeObject(responseText);
			if(json.results.Count != 0)
			{
				double latitude = double.Parse(json.results[0].geometry.location.lat.ToString(), System.Globalization.NumberStyles.Float);
				double longitude = double.Parse(json.results[0].geometry.location.lng.ToString(), System.Globalization.NumberStyles.Float);
				return new GeoCoordinate(latitude, longitude);
			}
			Debug.Assert(false);
			return null;
		}

		public static T RetryPattern<T>(Func<T> f, string error_msg)// throws after 10 attemps fails
		{
			Exception last_ex = null;
			for(int i = 0; i < 10; i++)
			{
				try
				{
					return f();
				}
				catch(Exception ex)
				{
					last_ex = ex;
					Thread.Sleep(TimeSpan.FromSeconds(2));
				}
			}
			throw new Exception(error_msg, last_ex);
		}

		private static Random rng = new Random();  

		public static IList<T> Shuffle<T>(this IList<T> list)  
		{  
			int n = list.Count;  
			while (n > 1) {  
				n--;  
				int k = rng.Next(n + 1);  
				T value = list[k];  
				list[k] = list[n];  
				list[n] = value;  
			}
			return list;
		}

		#region Mailing
		public static void SendMail(MailMessage message)
		{
			using(SmtpClient smtp = new SmtpClient("smtp.umbler.com", 587))
			{
				smtp.UseDefaultCredentials = false;
				smtp.Credentials = new NetworkCredential("ramon@misoftware.com.br", "atfi3fs9");
				smtp.Send(message);
			}

			using(SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
			{
				smtp.UseDefaultCredentials = false;
				smtp.EnableSsl = true;
				smtp.Credentials = new NetworkCredential("midiway.sc@gmail.com", "netdpm");
				smtp.Send(message);
			}
		}

		public static void SendTheMasterMail(string body, string subject)// messages are
		{
			MailMessage message = new MailMessage();
			message.To.Add("ramon@misoftware.com.br");
			message.Subject = subject;
			message.From = new MailAddress("ramon@misoftware.com.br");
			message.Body = body;
			SendMail(message);
		}

		public static void SendMailLogException(Exception ex)
		{
#if DEBUG
			Debug.Assert(false);
#else
			MailMessage message = new MailMessage();
			message.To.Add("ramon@misoftware.com.br");
			message.Subject = "DesignArsenal SITE - Exception";
			message.From = new MailAddress("ramon@misoftware.com.br");
			message.Body = ex.ToString();
			SendMail(message);
#endif
		}
		#endregion
	}

	public class ToStringJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return true;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString());
		}

		public override bool CanRead
		{
			get { return false; }
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}