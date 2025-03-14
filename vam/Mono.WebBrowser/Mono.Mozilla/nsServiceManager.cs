using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsServiceManager
{
	public static nsIServiceManager GetProxy(IWebBrowser control, nsIServiceManager obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIServiceManager).GUID, obj);
		return proxyForObject as nsIServiceManager;
	}
}
