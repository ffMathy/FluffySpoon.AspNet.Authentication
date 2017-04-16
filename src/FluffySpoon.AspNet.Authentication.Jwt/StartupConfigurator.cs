using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluffySpoon.AspNet.Authentication.Jwt
{
	public class StartupConfigurator
	{
		private readonly IJwtSettings _settings;

		public StartupConfigurator(
			IJwtSettings settings)
		{
			_settings = settings;
		}

		public void Configure(
			IApplicationBuilder app)
		{			
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = SigningKeyHelper.GenerateKeyFromSecret(_settings.SecretKey),
				ValidateIssuer = true,
				ValidIssuer = _settings.Issuer,
				ValidateAudience = true,
				ValidAudience = _settings.Audience,
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero
			};
			
			app.UseJwtBearerAuthentication(new JwtBearerOptions
			{
				AutomaticAuthenticate = true,
				AutomaticChallenge = true,
				TokenValidationParameters = tokenValidationParameters
			});
		}
	}
}
