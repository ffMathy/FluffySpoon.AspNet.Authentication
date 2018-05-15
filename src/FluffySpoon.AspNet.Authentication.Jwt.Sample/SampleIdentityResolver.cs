using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FluffySpoon.AspNet.Authentication.Jwt.Sample
{
  public class SampleIdentityResolver : IIdentityResolver
  {
    public async Task<ClaimsResult> GetClaimsAsync(Credentials credentials)
    {
		if(credentials == null)
			return new ClaimsResult();

      if (credentials.Username != "foo" || credentials.Password != "bar")
        return null;

      return new ClaimsResult()
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
    }
  }
}
