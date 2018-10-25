# Usage example
## Writing the code needed
### Configure the services
Add the following code to your `Startup` class' `ConfigureServices` method with real values instead of the sample values:

```csharp
services.AddFluffySpoonJwt<MySampleIdentityResolver>(
    new JwtSettings() {
		Audience = "https://www.example.com",
		Expiration = TimeSpan.FromDays(30),
		Issuer = "FluffySpoon sample",
		SecretKey = "very secret - such wow"
    });
```

- `MySampleIdentityResolver` is explained below.
- Audience is the URL of your website or a name specifying who the token is issued to.
- Expiration is how fast the token expires. When a token expires, the `IIdentityResolver` (in the above example `MySampleIdentityResolver`) is asked to validate the user's credentials again, resulting in a newly created token.
- Issuer is the name of whatever/whoever is issuing the token.
- Secret key is a static key that you generate once and specify, used for signing the token. _Do not distribute this to the client.

### Make an identity resolver
The `MySampleIdentityResolver` class (seen below) is in this case is a class that authenticates a user and decides what claims and roles that user has. The implementation in this case will authenticate if the username given is "foo" and the password given is "bar", and let the server know that this user is both an "Administrator" and a "User". Furthermore, it will generate some claims that are sent to the client.

```csharp
public class SampleIdentityResolver : IIdentityResolver
{
	public async Task<ClaimsResult> GetClaimsAsync(Credentials credentials)
	{
		if (credentials.Username != "foo" || credentials.Password != "bar")
			return null;

		return new ClaimsResult()
		{
			Roles = new[]
			{
				"Administrator",
				"User"
			},
			Claims = new Dictionary<string, string>()
			{
				{ "email", "foo@bar.com" },
				{ "first_name", "Mathias" },
				{ "last_name", "Lorenzen" }
			}
		};
	}
}
```

### Inject the middleware
Finally, inject the middleware in the `Startup` class' `Configure` method as such:

```csharp
public void Configure()
{
	app.UseFluffySpoonJwt();
}
```

## Fetching a new token
To fetch a new token, request any page (even non-existent ones) on your server, with a `Authorization` header using the value `FluffySpoon ABC` where `ABC` is a Base64 encoded JSON blob containing a `Username` and a `Password` property.

Let's say we want to authenticate with the username "foo" and the password "bar".

1. We create a JSON blob containing the `Username` and `Password` property: `{ Username: "foo", Password: "bar" }`.
2. We Base64 encode it: `eyBVc2VybmFtZTogImZvbyIsIFBhc3N3b3JkOiAiYmFyIiB9`.
3. We send that to a random URL (for instance `https://example.com/get-me-a-new-token-or-whatever`) with the header `Authorization: FluffySpoon eyBVc2VybmFtZTogImZvbyIsIFBhc3N3b3JkOiAiYmFyIiB9`.
4. The server responds with a `Token` header like `Token: MYTOKEN` where `MYTOKEN` is the generated JWT token for the client.
5. In future requests, we add the header `Authorization: Bearer MYTOKEN` to authenticate ourselves.
6. Should the token expire, the server will send a new one automatically in the `Token` header.