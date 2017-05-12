using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Internal.Http;
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
            string token;
            if (context?.User?.Claims?.Any() == true)
            {
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
            }
            else
            {
                token = generator.GenerateToken(
                  new Claim("fluffy-spoon.authentication.jwt.isAnonymous", "true"));
            }

            context
              .Response
              .Headers
              .Add("Token", token);

            if (context.Items.ContainsKey(Constants.MiddlewareTokenPassingKey))
            {
                context.Response.StatusCode = StatusCodes.Status204NoContent;
                return;
            }

            await _next(context);
        }

    }
}
