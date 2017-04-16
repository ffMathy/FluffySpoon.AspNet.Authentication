using System;

namespace FluffySpoon.AspNet.Authentication.Jwt
{
	public interface IJwtSettings
	{
		string Audience { get; set; }
		string Issuer { get; set; }
		string SecretKey { get; set; }

		TimeSpan Expiration { get; set; }
	}
}