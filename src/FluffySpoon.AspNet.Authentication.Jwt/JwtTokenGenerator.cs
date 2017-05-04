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
  public class JwtTokenGenerator : IJwtTokenGenerator
  {
    private readonly IIdentityResolver _identityResolver;
    private readonly IJwtSettings _settings;

    public JwtTokenGenerator(
        IIdentityResolver identityResolver,
        IJwtSettings settings)
    {
      _settings = settings;
      _identityResolver = identityResolver;
    }

    public string GenerateToken(
      params Claim[] claims)
    {
      var now = DateTime.UtcNow;

      var claimsList = new List<Claim>();
      claimsList.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
      claimsList.Add(
        new Claim(
          JwtRegisteredClaimNames.Iat,
          new DateTimeOffset(now)
              .ToUniversalTime()
              .ToUnixTimeSeconds()
              .ToString(),
          ClaimValueTypes.Integer64));

      var jwt = GenerateJwtToken(now, claims);
      var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
      return encodedJwt;
    }

    private JwtSecurityToken GenerateJwtToken(DateTime now, IEnumerable<Claim> claims)
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
