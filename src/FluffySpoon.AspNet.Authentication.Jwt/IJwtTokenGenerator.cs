using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FluffySpoon.AspNet.Authentication.Jwt
{
  public interface IJwtTokenGenerator
  {
    string GenerateToken(params Claim[] claims);
  }
}