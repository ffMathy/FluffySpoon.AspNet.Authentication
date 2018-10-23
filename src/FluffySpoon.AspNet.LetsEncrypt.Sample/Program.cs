using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FluffySpoon.AspNet.LetsEncrypt.Sample
{
	public class Program
	{
		public const string Domain = "64c19176.ngrok.io";

		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureLogging(l => l.AddConsole())
				.UseKestrel(kestrelOptions =>
				{
					kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
					{

					});
				})
				.UseUrls(
					"http://" + Domain,
					"https://" + Domain)
				.UseStartup<Startup>();
	}
}
