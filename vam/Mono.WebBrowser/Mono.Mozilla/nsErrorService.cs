using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsErrorService
{
	public static nsIErrorService GetProxy(IWebBrowser control, nsIErrorService obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIErrorService).GUID, obj);
		return proxyForObject as nsIErrorService;
	}
}
