using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluffySpoon.AspNet.Authentication.Jwt
{
	public static class RegistrationExtensions
	{
		public static void AddFluffySpoonJwt(
		  this IServiceCollection services,
		  IIdentityResolver identityResolver,
		  IJwtSettings jwtSettings)
		{
			var tokenGenerator = new JwtTokenGenerator(
			  identityResolver,
			  jwtSettings);
			services.AddSingleton<IJwtTokenGenerator>(tokenGenerator);

			services.AddSingleton(jwtSettings);
			services.AddSingleton(identityResolver);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = SigningKeyHelper.GenerateKeyFromSecret(jwtSettings.SecretKey),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(options =>
				{
					options.SaveToken = false;
					options.TokenValidationParameters = tokenValidationParameters;
					options.Events = new JwtBearerEvents()
					{
						OnMessageReceived = (context) =>
						{
							const string tokenKey = "fluffy-spoon.authentication.jwt.token";
							if (context.HttpContext.Items.ContainsKey(tokenKey))
							{
								context.Token = (string)context.HttpContext.Items[tokenKey];
							}
							return Task.CompletedTask;
						}
					};
				});
        }

		public static void UseFluffySpoonJwt(
			this IApplicationBuilder app)
		{
            app.UseMiddleware<PreAuthorizationMiddleware>();

			app.UseMiddleware<PostAuthorizationMiddleware>();
		}
	}
}
