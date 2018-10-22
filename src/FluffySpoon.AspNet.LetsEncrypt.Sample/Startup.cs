using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Certes;
using Certes.Acme;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FluffySpoon.AspNet.LetsEncrypt.Sample
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			string challengeToken = null;

			app.Run(async (context) =>
			{
				if(context.Request.Path.ToString().StartsWith("/.well-known/acme-challenge")) {
					await context.Response.WriteAsync(challengeToken);
					return;
				}

				AcmeContext acme;
				if(!File.Exists("key.dat")) {
					acme = new AcmeContext(WellKnownServers.LetsEncryptStagingV2);
					await acme.NewAccount("foo", true);

					var pemKey = acme.AccountKey.ToPem();
					File.WriteAllText("key.dat", pemKey);
				} else
				{
					var accountKey = KeyFactory.FromPem(File.ReadAllText("key.dat"));
					acme = new AcmeContext(WellKnownServers.LetsEncryptStagingV2, accountKey);

					await acme.Account();
				}

				var order = await acme.NewOrder(new[] { "stuff.ngrok.io" });

				var authz = (await order.Authorizations()).First();
				var challenge = await authz.Http();
				challengeToken = challenge.KeyAuthz;

				await challenge.Validate();

				await context.Response.WriteAsync("Hello World!");
			});
		}
	}
}
