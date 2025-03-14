using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsIOService
{
	public static nsIIOService GetProxy(IWebBrowser control, nsIIOService obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIIOService).GUID, obj);
		return proxyForObject as nsIIOService;
	}
}
