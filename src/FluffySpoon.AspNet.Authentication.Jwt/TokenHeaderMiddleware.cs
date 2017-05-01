using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace FluffySpoon.AspNet.Authentication.Jwt
{
  class TokenHeaderMiddleware
  {
    private readonly RequestDelegate _next;

    public TokenHeaderMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public Task Invoke(HttpContext context)
    {
      
      return this._next(context);
    }

  }
}
