using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsWebBrowserStream
{
	public static nsIWebBrowserStream GetProxy(IWebBrowser control, nsIWebBrowserStream obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIWebBrowserStream).GUID, obj);
		return proxyForObject as nsIWebBrowserStream;
	}
}
