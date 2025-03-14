using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsObserver
{
	public static nsIObserver GetProxy(IWebBrowser control, nsIObserver obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIObserver).GUID, obj);
		return proxyForObject as nsIObserver;
	}
}
