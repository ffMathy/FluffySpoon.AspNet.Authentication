using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluffySpoon.AspNet.Authentication.Jwt.Sample
{
    public static class SampleJwtSettingsHelper
    {
		public static IJwtSettings GenerateSettings()
		{
			return new JwtSettings()
			{
				Audience = "https://www.example.com",
				Expiration = TimeSpan.FromDays(30),
				Issuer = "FluffySpoon sample",
				SecretKey = "very secret - such wow"
			};
		}
    }
}
