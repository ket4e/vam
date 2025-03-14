using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsTimer
{
	public static nsITimer GetProxy(IWebBrowser control, nsITimer obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsITimer).GUID, obj);
		return proxyForObject as nsITimer;
	}
}
