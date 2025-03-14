using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsURI
{
	public static nsIURI GetProxy(IWebBrowser control, nsIURI obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIURI).GUID, obj);
		return proxyForObject as nsIURI;
	}
}
