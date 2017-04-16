using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FluffySpoon.AspNet.Authentication.Jwt.Sample
{
	public class SampleIdentityResolver : IIdentityResolver
	{
		public Task<ClaimsIdentity> GetClaimsAsync(Credentials credentials)
		{
			if (credentials.Username != "foo" || credentials.Password != "bar")
			{
				return Task.FromResult<ClaimsIdentity>(null);
			}

			var identity = new ClaimsIdentity(new[] {
					new Claim("email", "foo@bar.com"),
					new Claim(ClaimTypes.Name, "foo@bar.com"),
					new Claim("first_name", "Foo"),
					new Claim("last_name", "Stuff"),
					new Claim(ClaimTypes.Role, "Administrator"),
					new Claim(ClaimTypes.Role, "User")
				},
				ClaimTypes.AuthenticationMethod,
				ClaimTypes.Name,
				ClaimTypes.Role);
			return Task.FromResult(identity);
		}
	}
}
