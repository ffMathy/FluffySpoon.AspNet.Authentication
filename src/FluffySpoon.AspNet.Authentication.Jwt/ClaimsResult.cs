using System;
using System.Collections.Generic;
using System.Text;

namespace FluffySpoon.AspNet.Authentication.Jwt
{
  public class ClaimsResult
  {
    public string[] Roles { get; set; }
    public IDictionary<string, string> Claims { get; set; }

    public ClaimsResult()
    {
      Roles = new string[0];
      Claims = new Dictionary<string, string>();
    }
  }
}