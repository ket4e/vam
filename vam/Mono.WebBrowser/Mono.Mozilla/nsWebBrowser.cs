using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsWebBrowser
{
	public static nsIWebBrowser GetProxy(IWebBrowser control, nsIWebBrowser obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIWebBrowser).GUID, obj);
		return proxyForObject as nsIWebBrowser;
	}
}
