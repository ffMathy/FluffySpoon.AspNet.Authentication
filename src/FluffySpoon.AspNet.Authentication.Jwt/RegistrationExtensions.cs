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
    }

    public static void UseFluffySpoonJwt(
        this IApplicationBuilder app,
        IJwtSettings settings)
    {
      var tokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = SigningKeyHelper.GenerateKeyFromSecret(settings.SecretKey),
        ValidateIssuer = true,
        ValidIssuer = settings.Issuer,
        ValidateAudience = true,
        ValidAudience = settings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
      };

      app.UseMiddleware<PreAuthorizationMiddleware>();

      app.UseJwtBearerAuthentication(new JwtBearerOptions
      {
        AutomaticAuthenticate = true,
        AutomaticChallenge = true,
        SaveToken = false,
        TokenValidationParameters = tokenValidationParameters,
        Events = new JwtBearerEvents()
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
        }
      });

      app.UseMiddleware<PostAuthorizationMiddleware>();
    }
  }
}
