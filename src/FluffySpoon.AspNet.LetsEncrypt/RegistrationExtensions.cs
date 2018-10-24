using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FluffySpoon.AspNet.LetsEncrypt
{
	public static class RegistrationExtensions
	{
		public static void AddFluffySpoonLetsEncrypt(
		  this IServiceCollection services,
		  LetsEncryptOptions options)
		{
			AddFluffySpoonLetsEncrypt(services, options, 
				new FileCertificatePersistenceStrategy("FluffySpoonAspNetLetsEncryptCertificate"));
		}
		
		public static void AddFluffySpoonLetsEncrypt(
		  this IServiceCollection services,
		  LetsEncryptOptions options,
		  ICertificatePersistenceStrategy certificatePersistenceStrategy)
		{
			AddFluffySpoonLetsEncrypt(services, options,
				(p) => certificatePersistenceStrategy);
		}

		public static void AddFluffySpoonLetsEncrypt(
		  this IServiceCollection services,
		  LetsEncryptOptions options,
		  Func<IServiceProvider, ICertificatePersistenceStrategy> certificatePersistenceStrategyFactory)
		{
			services.AddSingleton<LetsEncryptCertificateContainer>();

			services.AddSingleton(options);
			services.AddSingleton(certificatePersistenceStrategyFactory);

			services.AddHostedService<LetsEncryptRenewalHostedService>();
		}

		public static void UseFluffySpoonLetsEncrypt(
			this IApplicationBuilder app)
		{
			app.UseMiddleware<LetsEncryptMiddleware>();
		}
	}
}
