using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsURIContentListener
{
	public static nsIURIContentListener GetProxy(IWebBrowser control, nsIURIContentListener obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIURIContentListener).GUID, obj);
		return proxyForObject as nsIURIContentListener;
	}
}
