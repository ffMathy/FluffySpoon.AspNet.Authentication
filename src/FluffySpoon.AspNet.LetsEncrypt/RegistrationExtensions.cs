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
		  LetsEncryptOptions options,
		  ICertificatePersistenceStrategy certificatePersistenceStrategy = null)
		{
			certificatePersistenceStrategy = 
				certificatePersistenceStrategy ??
				new FileCertificatePersistenceStrategy("FluffySpoonAspNetLetsEncryptCertificate");

			services.AddSingleton<LetsEncryptCertificateContainer>();

			services.AddSingleton(options);
			services.AddSingleton(certificatePersistenceStrategy);

			services.AddHostedService<LetsEncryptRenewalHostedService>();
        }

		public static void UseFluffySpoonLetsEncrypt(
			this IApplicationBuilder app)
		{
            app.UseMiddleware<LetsEncryptMiddleware>();
		}
	}
}
