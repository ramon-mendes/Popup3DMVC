using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Popup3DMVC.Classes;

namespace Popup3DMVC
{
	public class Startup
	{
		public static IHostingEnvironment _env { get; private set; }
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration, IHostingEnvironment env, IServiceProvider serviceProvider)
		{
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture =
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentUICulture = new CultureInfo("en-us");

			ValidatorOptions.LanguageManager.Culture = new CultureInfo("pt-br");

			Configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			/*services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });*/


			services.AddMvc();//.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			services.AddDistributedMemoryCache();
			services.AddSession();
			services.AddScoped<IViewRenderService, ViewRenderService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			_env = env;

			//if(env.IsDevelopment())
			if(true)
			{
				app.UseStatusCodePages();
				//app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
				app.UseHttpsRedirection();
			}

			app.UseStaticFiles();
			app.UseSession();
			//app.UseCookiePolicy();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}

		public static string MapPath(string path)
		{
			if(path[0] == '~')
				return _env.WebRootPath + '/' + path.Substring(1);
			return _env.ContentRootPath + '/' + path;
		}
	}
}
