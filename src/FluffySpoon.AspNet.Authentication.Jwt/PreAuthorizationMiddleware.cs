using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Internal.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FluffySpoon.AspNet.Authentication.Jwt
{
  class PreAuthorizationMiddleware
  {
    private readonly RequestDelegate _next;

    public PreAuthorizationMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task Invoke(
      HttpContext context,
      IJwtTokenGenerator generator,
      IIdentityResolver identityResolver,
      ILogger<PreAuthorizationMiddleware> logger)
    {
      var requestHeaders = (FrameRequestHeaders)context.Request.Headers;
      var authorization = requestHeaders.HeaderAuthorization.SingleOrDefault();

      const string authorizationPrefix = "FluffySpoon ";
      if (authorization != null && authorization.StartsWith(authorizationPrefix))
      {
        Credentials credentials;

        try
        {
          var credentialsCode = authorization.Substring(authorizationPrefix.Length);
          var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(credentialsCode));

          credentials = JsonConvert.DeserializeObject<Credentials>(decoded);
        }
        catch
        {
          logger.LogWarning("The given FluffySpoon authorization header was of an invalid format.");
          await _next(context);
          return;
        }

        var claimsResult = await identityResolver.GetClaimsAsync(credentials);

        var claims = claimsResult
          .Claims
          .Select(x => new Claim(
            x.Key,
            x.Value))
          .ToList();
        if (claims.Any(x => x.ValueType == ClaimTypes.Role))
        {
          throw new InvalidOperationException("Can't provide roles through claims. Use the roles array instead of the claims result.");
        }

        foreach (var role in claimsResult.Roles)
        {
          claims.Add(new Claim(
            ClaimTypes.Role,
            role));
        }

        claims.Add(new Claim(
          JwtRegisteredClaimNames.Sub,
          credentials.Username));
        claims.Add(new Claim(
          "fluffy-spoon.authentication.jwt.username",
          credentials.Username));

        var token = generator.GenerateToken(claims.ToArray());
        context.Items.Add("fluffy-spoon.authentication.jwt.token", token);
      }

      await _next(context);
    }

  }
}
