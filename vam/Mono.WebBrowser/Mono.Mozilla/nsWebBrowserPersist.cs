using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsWebBrowserPersist
{
	public static nsIWebBrowserPersist GetProxy(IWebBrowser control, nsIWebBrowserPersist obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIWebBrowserPersist).GUID, obj);
		return proxyForObject as nsIWebBrowserPersist;
	}
}
