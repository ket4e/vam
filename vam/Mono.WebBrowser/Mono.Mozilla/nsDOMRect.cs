using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMRect
{
	public static nsIDOMRect GetProxy(IWebBrowser control, nsIDOMRect obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMRect).GUID, obj);
		return proxyForObject as nsIDOMRect;
	}
}
