using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace FluffySpoon.AspNet.Authentication.Jwt
{
    public static class FluffySpoonJwt
    {
	    public static string GetJwtTokenFromHttpContext(this HttpContext httpContext)
	    {
		    return (string)httpContext.Items[Constants.MiddlewareTokenPassingKey];
	    }

	    public static bool IsPrincipalAnonymous(
		    this ClaimsPrincipal claimsPrincipal)
	    {
		    return !claimsPrincipal.HasClaim(x => x.Type == JwtRegisteredClaimNames.Sub);
	    }
    }
}
