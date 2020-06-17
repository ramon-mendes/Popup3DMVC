using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Popup3DMVC;
using Popup3DMVC.Classes;
using Popup3DMVC.DAL;
using Popup3DMVC.Models;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
		public static IWebHost BuildWebHost() =>
			WebHost.CreateDefaultBuilder()
				.UseContentRoot("D:\\Popup3DMVC\\Popup3DMVC")
				.UseEnvironment("development")
				.UseStartup<Startup>()
				.Build();

		[TestMethod]
        public void TestMethod1()
        {
			BuildWebHost();

			using(var db = new P3DContext())
			{
				var model = new PedidoModel()
				{
					Upload = db.Uploads.FindAll().ToList()[0],
					Checkout = new CheckoutModel()
				};
				model.valor_total = 200;
				model.Checkout.FillDemoAddrData();
				model.Checkout.nome = "Joao Bosco";
				model.Checkout.email = "joao@bosco.com";

				PagSeguro.GenerateToken(model);
			}
		}
    }
}