using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluffySpoon.AspNet.Authentication.Jwt
{
    public static class RegistrationExtensions
    {
		public static void UseFluffySpoonJwt(
			this IApplicationBuilder app,
			IJwtSettings settings)
		{
			var configurator = new StartupConfigurator(settings);
			configurator.Configure(app);
		}
    }
}
