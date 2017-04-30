using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FluffySpoon.AspNet.Authentication.Jwt
{
  public class JwtTokenGenerator
  {
    private readonly JsonSerializerSettings _serializerSettings;

    private readonly IIdentityResolver _identityResolver;
    private readonly IJwtSettings _settings;

    public JwtTokenGenerator(
        IIdentityResolver identityResolver,
        IJwtSettings settings)
    {
      _settings = settings;
      _identityResolver = identityResolver;
      _serializerSettings = new JsonSerializerSettings
      {
        Formatting = Formatting.Indented
      };
    }

    private async Task<IReadOnlyCollection<Claim>> GetClaimsAsync(
        Credentials credentials,
        DateTime requestTime)
    {
      var claims = new List<Claim>();

      var claimsResult = await _identityResolver.GetClaimsAsync(credentials);
      if (claimsResult == null)
        return claims;

      if (claimsResult.Claims.Any(x => x.Key == ClaimTypes.Role))
      {
        throw new InvalidOperationException("Role claims should be set via the roles property.");
      }

      claims.AddRange(new[]
      {
          new Claim(JwtRegisteredClaimNames.Sub, credentials.Username.ToLower()),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
          new Claim(JwtRegisteredClaimNames.Iat,
              new DateTimeOffset(requestTime)
                  .ToUniversalTime()
                  .ToUnixTimeSeconds()
                  .ToString(),
              ClaimValueTypes.Integer64)
      });

      foreach (var role in claimsResult.Roles)
      {
        claims.Add(new Claim(
          ClaimTypes.Role,
          role));
      }

      claims.AddRange(claimsResult
          .Claims
          .Select(x => new Claim(
            x.Key,
            x.Value)));

      return claims;
    }

    public async Task EmitTokenInResponseAsync(
        HttpResponse response,
        Credentials credentials)
    {
      var now = DateTime.UtcNow;

      var claims = await GetClaimsAsync(credentials, now);
      if (claims.Count == 0)
      {
        response.StatusCode = 403;
        await response.WriteAsync("Forbidden");
        return;
      }

      var jwt = GenerateJwtToken(now, claims);
      await EmitTokenAsync(response, jwt);
    }

    private async Task EmitTokenAsync(HttpResponse response, JwtSecurityToken jwt)
    {
      var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

      var jwtResponse = new
      {
        access_token = encodedJwt,
        expires_in = (int)_settings.Expiration.TotalSeconds
      };

      response.ContentType = "application/json";
      await response.WriteAsync(
          JsonConvert.SerializeObject(
              jwtResponse,
              _serializerSettings));
    }

    private JwtSecurityToken GenerateJwtToken(DateTime now, IReadOnlyCollection<Claim> claims)
    {
      var issuerSigningKey = SigningKeyHelper.GenerateKeyFromSecret(_settings.SecretKey);
      var signingCredentials = new SigningCredentials(
          issuerSigningKey,
          SecurityAlgorithms.HmacSha256);

      var jwt = new JwtSecurityToken(
          issuer: _settings.Issuer,
          audience: _settings.Audience,
          claims: claims,
          notBefore: now,
          expires: now.Add(_settings.Expiration),
          signingCredentials: signingCredentials);
      return jwt;
    }
  }
}
