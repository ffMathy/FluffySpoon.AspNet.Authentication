using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FluffySpoon.AspNet.Authentication.Jwt
{
	class PreAuthorizationMiddleware
	{
		private readonly RequestDelegate _next;

		public PreAuthorizationMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(
		  HttpContext context,
		  IJwtTokenGenerator generator,
		  IIdentityResolver identityResolver,
		  ILogger<PreAuthorizationMiddleware> logger)
		{
			var authorization = context.Request.Headers.SingleOrDefault(x => x.Key == "Authorization").Value.SingleOrDefault();

			Credentials credentials = null;

			const string fluffySpoonAuthorizationPrefix = "FluffySpoon";
			const string tokenAuthorizationPrefix = "Bearer";
			if (authorization != null)
			{
				if (authorization.StartsWith(fluffySpoonAuthorizationPrefix))
				{
					try
					{
						var credentialsCode = authorization.Substring(fluffySpoonAuthorizationPrefix.Length).Trim();
						if (!string.IsNullOrWhiteSpace(credentialsCode))
						{
							var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(credentialsCode));
							credentials = JsonConvert.DeserializeObject<Credentials>(decoded);
						}
					}
					catch
					{
						logger.LogWarning("The given FluffySpoon authorization header was of an invalid format.");
						await _next(context);
						return;
					}
				} else if (authorization.StartsWith(tokenAuthorizationPrefix))
				{
					await _next(context);
					return;
				}
			}

			var claimsResult = await identityResolver.GetClaimsAsync(credentials);
			if (claimsResult == null)
			{
				await _next(context);
				return;
			}

			var claims = claimsResult
			  .Claims
			  .Select(x => new Claim(
				x.Key,
				x.Value))
			  .ToList();
			if (claims.Any(x => x.ValueType == ClaimTypes.Role))
			{
				throw new InvalidOperationException("Can't provide roles through claims. Use the roles array instead of the claims result.");
			}

			foreach (var role in claimsResult.Roles)
			{
				claims.Add(new Claim(
				  ClaimTypes.Role,
				  role));
			}

			if (credentials != null)
				claims.Add(new Claim(
				  JwtRegisteredClaimNames.Sub,
				  credentials.Username));

			var token = generator.GenerateToken(claims.ToArray());
			context.Items.Add(Constants.MiddlewareTokenPassingKey, token);

			await _next(context);
		}
	}
}
