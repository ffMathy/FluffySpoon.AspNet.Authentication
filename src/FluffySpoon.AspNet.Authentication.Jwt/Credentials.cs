using System;
using System.Collections.Generic;
using System.Text;

namespace FluffySpoon.AspNet.Authentication.Jwt
{
    public class Credentials
    {
		public Credentials(
			string username,
			string password)
		{
			Username = username;
			Password = password;
		}

		public string Username { get; set; }
		public string Password { get; set; }
	}
}
