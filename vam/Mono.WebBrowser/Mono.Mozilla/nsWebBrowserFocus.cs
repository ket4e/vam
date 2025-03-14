using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsWebBrowserFocus
{
	public static nsIWebBrowserFocus GetProxy(IWebBrowser control, nsIWebBrowserFocus obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIWebBrowserFocus).GUID, obj);
		return proxyForObject as nsIWebBrowserFocus;
	}
}
