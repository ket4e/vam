using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMHTMLStyleElement
{
	public static nsIDOMHTMLStyleElement GetProxy(IWebBrowser control, nsIDOMHTMLStyleElement obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMHTMLStyleElement).GUID, obj);
		return proxyForObject as nsIDOMHTMLStyleElement;
	}
}
