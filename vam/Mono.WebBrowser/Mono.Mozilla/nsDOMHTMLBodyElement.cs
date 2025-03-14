using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMHTMLBodyElement
{
	public static nsIDOMHTMLBodyElement GetProxy(IWebBrowser control, nsIDOMHTMLBodyElement obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMHTMLBodyElement).GUID, obj);
		return proxyForObject as nsIDOMHTMLBodyElement;
	}
}
