The simplest LetsEncrypt setup for ASP .NET Core. No server configuration needed. 

`install-package FluffySpoon.AspNet.LetsEncrypt`

# Requirements
- Kestrel (which is default)
- ASP .NET Core 2.1+

# Usage example
If you want to try it yourself, you can also browse the sample project code here:

https://github.com/ffMathy/FluffySpoon.AspNet/tree/master/src/FluffySpoon.AspNet.LetsEncrypt.Sample

## Configure the services
Add the following code to your `Startup` class' `ConfigureServices` method with real values instead of the sample values:

```csharp
services.AddFluffySpoonLetsEncrypt(new LetsEncryptOptions()
{
	Email = "some-email@github.com", //LetsEncrypt will send you an e-mail here when the certificate is about to expire
	UseStaging = false, //switch to true for testing
	Domains = new[] { DomainToUse },
	TimeUntilExpiryBeforeRenewal = TimeSpan.FromDays(30), //renew automatically 30 days before expiry
	CertificateSigningRequest = new CsrInfo() //these are your certificate details
	{
		CountryName = "Denmark",
		Locality = "DK",
		Organization = "Fluffy Spoon",
		OrganizationUnit = "Hat department",
		State = "DK"
	}
});
```

## Inject the middleware
Finally, inject the middleware in the `Startup` class' `Configure` method as such:

```csharp
public void Configure()
{
	app.UseFluffySpoonLetsEncrypt();
}
```

## Configure Kestrel to look for the certificate
Finally, to make Kestrel automatically select the LetsEncrypt certificate, we must configure it as such. Here's an example `Program.cs`:

```csharp
WebHost.CreateDefaultBuilder(args)
	.UseKestrel(kestrelOptions => kestrelOptions.ConfigureHttpsDefaults(
			httpsOptions => httpsOptions.ServerCertificateSelector = 
				(c, s) => LetsEncryptCertificateContainer.Instance.Certificate))
	.UseUrls("http://" + DomainToUse, "https://" + DomainToUse)
	.UseStartup<Startup>();
```

Tada! Your application now supports SSL via LetsEncrypt, even from the first HTTPS request. It will even renew your certificate automatically in the background.

# Advanced configuration

## Decide how certificates are stored
Per default, the generated certificate will be stored in a file called `FluffySpoonAspNetLetsEncryptCertificate` (yes, no file extension). This can be changed by adding a second parameter to the `AddFluffySpoonLetsEncrypt` call, providing it with an `ICertificatePersistenceStrategy` that tells the middleware how to persist and retrieve the certificate as a byte array.

# Really?
Yes, really. This even works in an Azure App Service - technically any host that can host ASP .NET Core 2.1 applications can use this without issues.

I got tired of disappointing Azure support for LetsEncrypt, which currently requires a plugin and potentially hours of fiddling around just to get it working.