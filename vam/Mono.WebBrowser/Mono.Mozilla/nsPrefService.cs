using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsPrefService
{
	public static nsIPrefService GetProxy(IWebBrowser control, nsIPrefService obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIPrefService).GUID, obj);
		return proxyForObject as nsIPrefService;
	}
}
