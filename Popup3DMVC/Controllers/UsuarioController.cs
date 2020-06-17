using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Popup3DMVC.Classes;
using Popup3DMVC.Models;

namespace Popup3DMVC.Controllers
{
	public class UsuarioController : BaseController
    {
		// GET: /Usuario/Cadastrar
		public IActionResult Cadastrar(bool checkout = false)
		{
			if(Auth.IsLogged(HttpContext))
			{
				if(checkout)
				{
					return RedirectToAction("Checkout", "Imprimir");
				}
				Alert("Você entrou com sucesso!");
				return RedirectToAction("Index", "Home");
			}
			ViewBag.checkout = checkout;
			return View();
		}

		// POST: /Usuario/CadastrarAjaxCheck
		[HttpPost]
		public IActionResult CadastrarAjaxCheck(UICadastrarModel model)
		{
			var validation = new CadastroValidation(db).Validate(model);
			if(validation.IsValid)
				return Json(new { res = true });

			var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
			return Json(new
			{
				res = false,
				errors
			});
		}

		// POST: /Usuario/Cadastrar
		[HttpPost]
		public IActionResult Cadastrar(UICadastrarModel model, string redirect = null)
		{
			var validation = new CadastroValidation(db).Validate(model);
			if(!validation.IsValid)
				throw new Exception("Validation failure");

			db.Users.Insert(new UserModel
			{
				dt = DateTime.Now,
				nome = model.Nome,
				email = model.Email,
				pwd_hash = Auth.ComputeHash(model.Senha)
			});

			Auth.Login(HttpContext, model.Email, model.Senha, db);

			if(redirect != null)
				return Redirect(redirect);
			return RedirectToAction("Index", "Home");
		}

		public IActionResult EsqueciMinhaSenha() => View();

		// POST: /Usuario/Login
		[HttpPost]
		public IActionResult Login(string email, string pwd)
		{
			if(!Auth.Login(HttpContext, email, pwd, db))
			{
				Thread.Sleep(1500);
				return StatusCode(401);
			}
			return StatusCode(200);
		}

		// GET: /Usuario/Logout
		public IActionResult Logout()
		{
			Auth.Logout(HttpContext);
			return RedirectToAction("Index", "Home");
		}
    }
}