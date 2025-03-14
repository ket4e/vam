using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsProperties
{
	public static nsIProperties GetProxy(IWebBrowser control, nsIProperties obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIProperties).GUID, obj);
		return proxyForObject as nsIProperties;
	}
}
