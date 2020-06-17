using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Popup3DMVC.Classes;
using Popup3DMVC.Models;
using LiteDB;

namespace Popup3DMVC.Controllers
{
    public class ImprimirController : BaseController
	{
		const string SESSION_SELECTION = "PrintSelection";

		private IViewRenderService _viewRenderService;

		public ImprimirController(IViewRenderService vrs)
		{
			_viewRenderService = vrs;
		}

		private Utils.EnderecoCEP SetCEP_End(string cep)// throws -> you must catch and handle it
		{
			if(string.IsNullOrWhiteSpace(cep))
				return null;
			if(!Regex.IsMatch(cep, "\\d{5}[-]\\d{2}"))
				return null;

			Utils.EnderecoCEP end = null;
			Utils.RetryPattern(() => end = Utils.BuscaCEP(cep), "ERROR");
			
			HttpContext.Session.SetString("cep", cep);
			HttpContext.Session.SetString("end", JsonConvert.SerializeObject(end));
			return end;
		}

		private Utils.EnderecoCEP GetEndereco()
		{
			if(!HttpContext.Session.Keys.Contains("end"))
				return null;
			return JsonConvert.DeserializeObject<Utils.EnderecoCEP>(HttpContext.Session.GetString("end"));
		}

		// GET: /Imprimir/Upload/34
		public IActionResult Upload(string id)
		{
			var upload = db.Uploads.FindById(new ObjectId(id));
			if(upload == null)
				return NotFound();

			ViewBag.cep = null;
			if(HttpContext.Session.Keys.Contains("cep"))
				ViewBag.cep = HttpContext.Session.GetString("cep");
			return View("Details", upload);
		}

		// GET: /Imprimir/AjaxList
		public IActionResult AjaxList(string cep, string id_upload)
		{
			var upload = db.Uploads.FindById(new ObjectId(id_upload));
			try
			{
				var end = SetCEP_End(cep);
				if(end == null)
					return Json(new { ok = false, err = "CEP inválido" });

				var coord = Utils.RetryPattern(() => Utils.BuscaCoordEnd(end), "ERROR");
				var empresas = db.Empresas.FindAll().ToList();
				if(coord != null)
					empresas.OrderBy(p => p, new GeoDistComparer(coord)).ToList();

				List<PrinterUIModel> printers = new List<PrinterUIModel>();
				foreach(var e in empresas)
				{
					var impr = db.Impressoras.Include(i => i.Empresa).Find(i => i.Empresa.Id == e.Id);
					foreach(var i in impr)
					{
						var mats = db.Material.Include(m => m.Impressora).Include(m => m.Impressora.Empresa).Find(m => m.Impressora.Id == i.Id).ToList();
						foreach(var m in mats)
						{
							double valor_peca = m.PrecoPeca(upload);
							printers.Add(new PrinterUIModel(m, valor_peca));
						}
					}
				}

				string html = _viewRenderService.RenderToString("Imprimir/_PrinterList", printers);
				return Json(new { ok = true, html });
			}
			catch(Exception)
			{
				return Json(new { ok = false, err = "Falha no servidor" });
			}
		}

		// GET: /Imprimir/AjaxSelectItem
		public IActionResult AjaxSelectItem(string id_upload, string id_material, string cor)
		{
			HttpContext.Session.SetString(SESSION_SELECTION, JsonConvert.SerializeObject(new PrintSelection
			{
				id_upload = id_upload,
				id_material = id_material,
				cor = cor,
			}));
			return Json(new { ok = true });
		}

		private class PrintSelection
		{
			public string id_upload;
			public string id_material;
			public string cor;
		}


		private (double, double) CalculaFrete(string cep_origem, string cep_destino, double peso_kg)
		{
			var peso = Math.Round(peso_kg, 5).ToString();
			var res_sedex = new CorreiosAPI.CalcPrecoPrazoWSSoapClient(CorreiosAPI.CalcPrecoPrazoWSSoapClient.EndpointConfiguration.CalcPrecoPrazoWSSoap).CalcPrecoAsync("", "", "40010", cep_origem, cep_destino, peso, 1, 16, 15, 15, 0, "s", 200, "n").Result;
			var res_pac = new CorreiosAPI.CalcPrecoPrazoWSSoapClient(CorreiosAPI.CalcPrecoPrazoWSSoapClient.EndpointConfiguration.CalcPrecoPrazoWSSoap).CalcPrecoAsync("", "", "41106", cep_origem, cep_destino, peso, 1, 16, 15, 15, 0, "s", 200, "n").Result;
			var sedex = res_sedex.Servicos[0];
			var pac = res_pac.Servicos[0];
			if(sedex.Erro != "" || pac.Erro != "")
				throw new Exception("Erro ao calcular o valor do frete. Verifique o CEP.");
			return (double.Parse(pac.Valor, Consts.COMMA_NFI), double.Parse(sedex.Valor, Consts.COMMA_NFI));
		}

		// GET: /Imprimir/Checkout
		public IActionResult Checkout()
		{
			if(!HttpContext.Session.Keys.Contains(SESSION_SELECTION))
				return Redirect("/");
			if(!Auth.IsLogged(HttpContext))
				return Redirect("/Usuario/Cadastrar?checkout=true");
			var end = GetEndereco();
			if(end == null)
				return Redirect("/");

			var model = PedidoFromSession();
			return View(model);
		}

		private PedidoModel PedidoFromSession()
		{
			var selection = JsonConvert.DeserializeObject<PrintSelection>(HttpContext.Session.GetString(SESSION_SELECTION));
			var material = db.Material.IncludeAll().FindById(new ObjectId(selection.id_material));
			var upload = db.Uploads.FindById(new ObjectId(selection.id_upload));

			// frete
			var end = GetEndereco();
			var (pac, sedex) = CalculaFrete(material.Impressora.Empresa.end.cep, end.cep, upload.volume / 1000);// cm³ to Kg

			var model = new PedidoModel
			{
				dt = DateTime.Now,

				User = Auth.LoggedUser(HttpContext, db),
				Upload = upload,
				Material = material,
				Impressora = material.Impressora,
				Empresa = material.Impressora.Empresa,
				Checkout = new CheckoutModel()
				{
					CEP = end.cep,
					Rua = end.rua,
					Bairro = end.bairro,
					Cidade = end.cidade,
					UF = end.UF
				},

				cor = selection.cor,
				prazo_dias_envio = 3,
				prazo_dias_impressao = 3,
				valor_frete = sedex,
				valor_impressao = material.PrecoPeca(upload)
			};
			model.valor_total = model.valor_frete + model.valor_impressao;

			model.Checkout.Nome = HttpContext.LoggedUser(db).nome;
			model.Checkout.Email = HttpContext.LoggedUser(db).email;

			return model;
		}

		// GET: /Imprimir/CheckoutSetCEP
		public IActionResult CheckoutSetCEP(string cep)
		{
			try
			{
				SetCEP_End(cep);
			}
			catch(Exception)
			{
				Error("CEP inválido.");
			}
			return RedirectToAction("Checkout");
		}

		// POST: /Imprimir/CheckoutPagamento
		public IActionResult AjaxCheckoutPagamento(CheckoutModel checkout)
		{
			try
			{
				var validation = new CheckoutValidation().Validate(checkout);
				if(validation.IsValid)
				{
					PedidoModel pedido = PedidoFromSession();
					pedido.Checkout = checkout;
					db.Pedidos.Insert(pedido);

					int valor = (int)((pedido.valor_total) * 100);
					return Json(new
					{
						res = true,
						amount = valor.ToString(),
						/*customer = new
						{
							external_id = "123",
						}*/
					});
				}
				else
				{
					var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
					return Json(new
					{
						res = false,
						errors
					});
				}
			}
			catch(Exception)
			{
				return Json(new
				{
					res = false,
					errors = new[] { "Erro no servidor" }
				});
			}
		}
	}
}