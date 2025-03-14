using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMNSRange
{
	public static nsIDOMNSRange GetProxy(IWebBrowser control, nsIDOMNSRange obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMNSRange).GUID, obj);
		return proxyForObject as nsIDOMNSRange;
	}
}
