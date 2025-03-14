using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsStreamListener
{
	public static nsIStreamListener GetProxy(IWebBrowser control, nsIStreamListener obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIStreamListener).GUID, obj);
		return proxyForObject as nsIStreamListener;
	}
}
