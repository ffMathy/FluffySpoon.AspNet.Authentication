using Microsoft.AspNetCore.Http;
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
    class PostAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public PostAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
          HttpContext context,
          IJwtTokenGenerator generator)
        {
	        if (context?.Items?.Any() == true)
            {
	            string token;
	            if (context.Items.ContainsKey(Constants.MiddlewareTokenPassingKey))
                {
                    token = (string)context.Items[Constants.MiddlewareTokenPassingKey];
                }
                else
                {
                    token = generator.GenerateToken(context
                      .User
                      .Claims
                      .ToArray());
                }

                context
                  .Response
                  .Headers
                  .Add("Token", token);

                if (context.Items.ContainsKey(Constants.MiddlewareTokenPassingKey))
                {
                    context.Response.StatusCode = StatusCodes.Status201Created;
                    return;
                }
            }

            await _next(context);
        }

    }
}
