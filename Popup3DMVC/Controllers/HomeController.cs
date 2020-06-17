using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Popup3DMVC.Models;
using Popup3DMVC.Classes;
using LiteDB;
using FILE = System.IO.File;
using JSON = Newtonsoft.Json.JsonConvert;

namespace Popup3DMVC.Controllers
{
	public class HomeController : BaseController
	{
		public class FeaturedModel
		{
			public UploadModel upload { get; set; }
			public MaterialModel material { get; set; }
			public double menor_preco { get; set; }
		}

		static bool _first_req = true;

		public IActionResult Index()
		{
#if DEBUG
			if(_first_req)
			{
				_first_req = false;
				Auth.Login(HttpContext, "ramon@misoftware.com.br", "pcdcss", db);
				return Redirect("/");
			}
#endif

			var list = db.Uploads
				.Find(u => u.featured)
				.Select(u =>
				{
					var m = db.Material.FindAll().First();

					return new FeaturedModel()
					{
						upload = u,
						material = m,
						menor_preco = m.PrecoPeca(u)
					};
				})
				.ToList()
				.Shuffle()
				.ToList();

			ViewBag.json_featured = JSON.SerializeObject(list);
			return View(list);
		}

		public IActionResult Upload(IFormCollection form)
		{
			var stl = form.Files[0];
			var subpath = "/uploads/" + Guid.NewGuid().ToString() + ".stl";
			var filePath = Startup._env.WebRootPath + subpath;
			var fileStream = FILE.Create(filePath);

			var stream = stl.OpenReadStream();
			stream.Seek(0, System.IO.SeekOrigin.Begin);
			stream.CopyTo(fileStream);
			stream.Close();
			fileStream.Close();

			var model = new UploadModel()
			{
				dt = DateTime.Now,
				subpath = subpath,
				filename = stl.FileName,
			};

			model.FillStlInfo(filePath);

			var id = db.Uploads.Insert(model);

			return Json(new { id = id.AsString });
		}

		[HttpPost]
		public IActionResult Index(IFormCollection form, IFormFile stl)
		{
			var filepath = "/tmp/" + Guid.NewGuid().ToString() + ".bin";
			var path = Startup._env.WebRootPath + filepath;
			var fileStream = FILE.Create(path);

			var stream = stl.OpenReadStream();
			stream.Seek(0, System.IO.SeekOrigin.Begin);
			stream.CopyTo(fileStream);
			stream.Close();
			fileStream.Close();

			TempData["stlfile"] = filepath;
			return RedirectToAction("StlViewer", new { path = filepath });
		}

		// GET: /Home/Monetizar
		public void Monetizar() => View();

		// GET: /Home/Sobre
		public void Sobre() => Sobre();

		// GET: /Home/Contato
		public void Contato() => View();

		// GET: /Home/TermosUsuario
		public IActionResult TermosUsuario() => View();

		// GET: /Home/TermosPrinter
		public IActionResult TermosPrinter() => View();
	}
}