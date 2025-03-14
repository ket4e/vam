using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsSHistory
{
	public static nsISHistory GetProxy(IWebBrowser control, nsISHistory obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsISHistory).GUID, obj);
		return proxyForObject as nsISHistory;
	}
}
