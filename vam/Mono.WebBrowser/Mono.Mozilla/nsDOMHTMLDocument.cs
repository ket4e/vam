using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMHTMLDocument
{
	public static nsIDOMHTMLDocument GetProxy(IWebBrowser control, nsIDOMHTMLDocument obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMHTMLDocument).GUID, obj);
		return proxyForObject as nsIDOMHTMLDocument;
	}
}
