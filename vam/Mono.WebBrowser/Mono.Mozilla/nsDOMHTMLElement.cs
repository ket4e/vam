using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMHTMLElement
{
	public static nsIDOMHTMLElement GetProxy(IWebBrowser control, nsIDOMHTMLElement obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMHTMLElement).GUID, obj);
		return proxyForObject as nsIDOMHTMLElement;
	}
}
