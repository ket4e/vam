using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsWebBrowserChromeFocus
{
	public static nsIWebBrowserChromeFocus GetProxy(IWebBrowser control, nsIWebBrowserChromeFocus obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIWebBrowserChromeFocus).GUID, obj);
		return proxyForObject as nsIWebBrowserChromeFocus;
	}
}
