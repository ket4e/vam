using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsWebBrowserChrome
{
	public static nsIWebBrowserChrome GetProxy(IWebBrowser control, nsIWebBrowserChrome obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIWebBrowserChrome).GUID, obj);
		return proxyForObject as nsIWebBrowserChrome;
	}
}
