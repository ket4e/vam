using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsRequestObserver
{
	public static nsIRequestObserver GetProxy(IWebBrowser control, nsIRequestObserver obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIRequestObserver).GUID, obj);
		return proxyForObject as nsIRequestObserver;
	}
}
