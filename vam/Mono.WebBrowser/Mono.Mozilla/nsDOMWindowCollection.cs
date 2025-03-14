using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMWindowCollection
{
	public static nsIDOMWindowCollection GetProxy(IWebBrowser control, nsIDOMWindowCollection obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMWindowCollection).GUID, obj);
		return proxyForObject as nsIDOMWindowCollection;
	}
}
