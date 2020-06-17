using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Popup3DMVC.Classes;
using Popup3DMVC.DAL;
using FILE = System.IO.File;

namespace Popup3DMVC.Controllers
{
    public class BaseController : Controller
	{
		protected P3DContext db = new P3DContext();

		public BaseController()
		{
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			ViewBag.svgsprite = FILE.ReadAllText(Startup.MapPath("~/img/svg/icon-sprites.svg"));
			ViewBag.auth_logged = Auth.IsLogged(context.HttpContext);
			if(ViewBag.auth_logged)
			{
				var user = Auth.LoggedUser(context.HttpContext, db);
				ViewBag.auth_name = user.nome;
			}
			base.OnActionExecuting(context);
		}

		public override void OnActionExecuted(ActionExecutedContext context)
		{
			db.Dispose();
			base.OnActionExecuted(context);
		}

		public void Alert(string msg) => TempData["msg-alert"] = msg;
		public void Error(string msg) => TempData["msg-error"] = msg;
	}
}