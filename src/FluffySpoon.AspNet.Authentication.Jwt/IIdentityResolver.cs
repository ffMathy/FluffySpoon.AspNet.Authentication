using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FluffySpoon.AspNet.Authentication.Jwt
{
    public interface IIdentityResolver
    {
		/// <summary>
		/// 
		/// </summary>
		/// <returns>Null if access is denied.</returns>
		Task<ClaimsIdentity> GetClaimsAsync(Credentials credentials);
    }
}
