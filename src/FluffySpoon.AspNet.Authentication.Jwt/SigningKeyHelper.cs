using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluffySpoon.AspNet.Authentication.Jwt
{
    static class SigningKeyHelper
    {
		internal static SymmetricSecurityKey GenerateKeyFromSecret(string secret)
		{
			return new SymmetricSecurityKey(
				Encoding.ASCII.GetBytes(
					secret));
		}
	}
}
