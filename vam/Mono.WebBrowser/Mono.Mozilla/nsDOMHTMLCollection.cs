using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMHTMLCollection
{
	public static nsIDOMHTMLCollection GetProxy(IWebBrowser control, nsIDOMHTMLCollection obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMHTMLCollection).GUID, obj);
		return proxyForObject as nsIDOMHTMLCollection;
	}
}
