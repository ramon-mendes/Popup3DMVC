using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Popup3DMVC.Classes;

namespace Popup3DMVC.Controllers
{
	public class ContaController : BaseController
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if(!Auth.IsLogged(HttpContext))
			{
				context.Result = new NotFoundResult();
				return;
			}
			ViewBag.nome = Auth.LoggedUser(HttpContext, db).nome;
			base.OnActionExecuting(context);
		}

		public IActionResult Index()
		{
			return View("Sumario");
		}

		public IActionResult Compras()
		{
			var user = Auth.LoggedUser(HttpContext, db);
			var pedidos = db.Pedidos.IncludeAll().Find(p => p.User.Id == user.Id).OrderByDescending(p => p.dt).ToList();
			return View(pedidos);
		}

		public IActionResult Pedido(int id)
		{
			var pedido = db.Pedidos.IncludeAll().FindById(id);
			if(pedido == null)
				return NotFound();
			if(pedido.User.Id != Auth.LoggedUser(HttpContext, db).Id)
				return NotFound();

			var u = db.Uploads.FindById(pedido.Upload.Id);
			if(u == null)
				return NotFound();
			

			return View(pedido);
		}
	}
}