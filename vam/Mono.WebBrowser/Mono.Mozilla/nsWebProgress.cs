using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsWebProgress
{
	public static nsIWebProgress GetProxy(IWebBrowser control, nsIWebProgress obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIWebProgress).GUID, obj);
		return proxyForObject as nsIWebProgress;
	}
}
