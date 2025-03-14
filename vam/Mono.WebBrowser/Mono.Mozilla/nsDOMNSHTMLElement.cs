using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMNSHTMLElement
{
	public static nsIDOMNSHTMLElement GetProxy(IWebBrowser control, nsIDOMNSHTMLElement obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMNSHTMLElement).GUID, obj);
		return proxyForObject as nsIDOMNSHTMLElement;
	}
}
