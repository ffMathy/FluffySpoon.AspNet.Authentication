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
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureLogging(l => l.AddConsole())
				.UseKestrel(kestrelOptions =>
				{
					kestrelOptions.Listen(IPAddress.Parse("foo"), 80);
					kestrelOptions.Listen(IPAddress.Parse("foo"), 443, listenOptions =>
					{
						listenOptions.UseHttps(httpsOptions =>
						{
						});
					});
				})
				.UseStartup<Startup>();
	}
}
