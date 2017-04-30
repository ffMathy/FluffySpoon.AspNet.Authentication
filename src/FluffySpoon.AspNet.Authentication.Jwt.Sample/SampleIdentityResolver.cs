using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FluffySpoon.AspNet.Authentication.Jwt.Sample
{
  public class SampleIdentityResolver : IIdentityResolver
  {
    public Task<ClaimsResult> GetClaimsAsync(Credentials credentials)
    {
      if (credentials.Username != "foo" || credentials.Password != "bar")
      {
        return Task.FromResult<ClaimsResult>(null);
      }

      var result = new ClaimsResult()
      {
        Roles = new[]
        {
          "Administrator",
          "User"
        },
        Claims = new Dictionary<string, string>()
        {
          { "email", "foo@bar.com" },
          { "first_name", "Mathias" },
          { "last_name", "Lorenzen" }
        }
      };

      return Task.FromResult(result);
    }
  }
}
