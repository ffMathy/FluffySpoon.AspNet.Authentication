using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluffySpoon.AspNet.Authentication.Jwt
{
  public static class RegistrationExtensions
  {
    public static void AddFluffySpoonJwt(
      this IServiceCollection services,
      IIdentityResolver identityResolver)
    {
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

      app.UseJwtBearerAuthentication(new JwtBearerOptions
      {
        AutomaticAuthenticate = true,
        AutomaticChallenge = true,
        SaveToken = false,
        TokenValidationParameters = tokenValidationParameters
      });

      app.UseMiddleware<AuthorizationMiddleware>();
    }
  }
}
