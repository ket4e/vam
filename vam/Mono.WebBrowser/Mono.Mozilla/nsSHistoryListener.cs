using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsSHistoryListener
{
	public static nsISHistoryListener GetProxy(IWebBrowser control, nsISHistoryListener obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsISHistoryListener).GUID, obj);
		return proxyForObject as nsISHistoryListener;
	}
}
