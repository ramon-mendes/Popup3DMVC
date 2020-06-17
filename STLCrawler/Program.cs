using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace STLCrawler
{
	class Program
	{
		static string[] categories = new[]
		{
			"accessibility",
			"education",
			"electronics-computers",
			"miniworld",
			"brands-spare-parts",
			"upcycling",
			"Games",
			"fan-art",
			"home-garden",
			"cosplay",
			"sports-outdoor",
			"for-3d-printer-owners",
			"fashion-and-accessories",
			"Jewellery",
			"scantheworld",
			"toys-and-games",
		};

		static void Main(string[] args)
		{
			foreach(var item in categories)
			{
				using(HttpClient hc = new HttpClient())
				{
					var response = hc.GetAsync("https://www.myminifactory.com/category/" + item).Result;
					var html = response.Content.ReadAsStringAsync().Result;

					var doc_main = new HtmlDocument();
					doc_main.LoadHtml(html);

					var anchors = doc_main.QuerySelectorAll(".thumbnail a").ToList();
					Parallel.ForEach(anchors, (el_a) =>
					{
						var url = el_a.Attributes["href"].Value;
						response = hc.GetAsync("https://www.myminifactory.com" + url).Result;
						html = response.Content.ReadAsStringAsync().Result;

						var doc_model = new HtmlDocument();
						doc_model.LoadHtml(html);

						var el_form = doc_model.QuerySelector("#mymini_userinterfacebundle_download_submit");
						var dl_url = el_form.Attributes["action"].Value;

						response = hc.GetAsync("https://www.myminifactory.com" + dl_url).Result;
						var dl_redir_url = response.RequestMessage.RequestUri;
					});
				}
			}
		}
	}
}