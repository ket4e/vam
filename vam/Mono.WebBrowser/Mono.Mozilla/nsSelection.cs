using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsSelection
{
	public static nsISelection GetProxy(IWebBrowser control, nsISelection obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsISelection).GUID, obj);
		return proxyForObject as nsISelection;
	}
}
