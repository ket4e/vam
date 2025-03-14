using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMEntityReference
{
	public static nsIDOMEntityReference GetProxy(IWebBrowser control, nsIDOMEntityReference obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMEntityReference).GUID, obj);
		return proxyForObject as nsIDOMEntityReference;
	}
}
