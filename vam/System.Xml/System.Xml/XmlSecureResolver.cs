using System.Net;
using System.Security;
using System.Security.Policy;

namespace System.Xml;

public class XmlSecureResolver : XmlResolver
{
	private XmlResolver resolver;

	private PermissionSet permissionSet;

	public override ICredentials Credentials
	{
		set
		{
			resolver.Credentials = value;
		}
	}

	public XmlSecureResolver(XmlResolver resolver, Evidence evidence)
	{
		this.resolver = resolver;
		if (SecurityManager.SecurityEnabled)
		{
			permissionSet = SecurityManager.ResolvePolicy(evidence);
		}
	}

	public XmlSecureResolver(XmlResolver resolver, PermissionSet permissionSet)
	{
		this.resolver = resolver;
		this.permissionSet = permissionSet;
	}

	public XmlSecureResolver(XmlResolver resolver, string securityUrl)
	{
		this.resolver = resolver;
		if (SecurityManager.SecurityEnabled)
		{
			permissionSet = SecurityManager.ResolvePolicy(CreateEvidenceForUrl(securityUrl));
		}
	}

	public static Evidence CreateEvidenceForUrl(string securityUrl)
	{
		Evidence evidence = new Evidence();
		if (securityUrl != null && securityUrl.Length > 0)
		{
			try
			{
				Url id = new Url(securityUrl);
				evidence.AddHost(id);
			}
			catch (ArgumentException)
			{
			}
			try
			{
				Zone id2 = Zone.CreateFromUrl(securityUrl);
				evidence.AddHost(id2);
			}
			catch (ArgumentException)
			{
			}
			try
			{
				Site id3 = Site.CreateFromUrl(securityUrl);
				evidence.AddHost(id3);
			}
			catch (ArgumentException)
			{
			}
		}
		return evidence;
	}

	[System.MonoTODO]
	public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
	{
		if (SecurityManager.SecurityEnabled)
		{
			if (permissionSet == null)
			{
				throw new SecurityException(global::Locale.GetText("Security Manager wasn't active when instance was created."));
			}
			permissionSet.PermitOnly();
		}
		return resolver.GetEntity(absoluteUri, role, ofObjectToReturn);
	}

	public override Uri ResolveUri(Uri baseUri, string relativeUri)
	{
		return resolver.ResolveUri(baseUri, relativeUri);
	}
}
