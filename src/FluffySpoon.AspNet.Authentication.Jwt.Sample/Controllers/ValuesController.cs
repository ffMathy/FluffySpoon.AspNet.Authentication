using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FluffySpoon.AspNet.Authentication.Jwt.Sample.Controllers
{
  [Route("api/[controller]")]
  public class ValuesController : Controller
  {
    [HttpGet("secret-stuff")]
    [Authorize]
    public IEnumerable<string> GetSecretStuff()
    {
      return new string[] { "this is", "authenticated", "with JWT", ":)" };
    }

    [HttpGet("needs-admin")]
    [Authorize(Roles = "Administrator")]
    public string NeedsAdministratorRights()
    {
      return "This is secret";
    }

    [HttpGet("needs-user")]
    [Authorize(Roles = "User")]
    public string NeedsUserRights()
    {
      return "This is secret";
    }

    [HttpGet("needs-mutant")]
    [Authorize(Roles = "Mutant")]
    public string NeedsMutantRights()
    {
      return "This is secret";
    }

    [HttpGet("non-secret-stuff")]
    public string GetNonSecretStuff()
    {
      return "This is not authenticated with JWT";
    }
  }
}
