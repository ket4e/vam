using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMElement
{
	public static nsIDOMElement GetProxy(IWebBrowser control, nsIDOMElement obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMElement).GUID, obj);
		return proxyForObject as nsIDOMElement;
	}
}
