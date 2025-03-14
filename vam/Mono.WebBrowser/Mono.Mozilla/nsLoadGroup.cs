using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsLoadGroup
{
	public static nsILoadGroup GetProxy(IWebBrowser control, nsILoadGroup obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsILoadGroup).GUID, obj);
		return proxyForObject as nsILoadGroup;
	}
}
