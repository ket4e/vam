using System.Collections;
using System.Net;

namespace Mono.Http;

internal class NtlmClient : IAuthenticationModule
{
	private static Hashtable cache;

	public string AuthenticationType => "NTLM";

	public bool CanPreAuthenticate => false;

	static NtlmClient()
	{
		cache = new Hashtable();
	}

	public Authorization Authenticate(string challenge, WebRequest webRequest, ICredentials credentials)
	{
		if (credentials == null || challenge == null)
		{
			return null;
		}
		string text = challenge.Trim();
		int num = text.ToLower().IndexOf("ntlm");
		if (num == -1)
		{
			return null;
		}
		num = text.IndexOfAny(new char[2] { ' ', '\t' });
		text = ((num == -1) ? null : text.Substring(num).Trim());
		if (!(webRequest is HttpWebRequest httpWebRequest))
		{
			return null;
		}
		lock (cache)
		{
			Mono.Http.NtlmSession ntlmSession = (Mono.Http.NtlmSession)cache[httpWebRequest.RequestUri];
			if (ntlmSession == null)
			{
				ntlmSession = new Mono.Http.NtlmSession();
				cache.Add(httpWebRequest.RequestUri, ntlmSession);
			}
			return ntlmSession.Authenticate(text, webRequest, credentials);
		}
	}

	public Authorization PreAuthenticate(WebRequest webRequest, ICredentials credentials)
	{
		return null;
	}
}
