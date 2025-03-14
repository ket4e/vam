using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMDocumentStyle
{
	public static nsIDOMDocumentStyle GetProxy(IWebBrowser control, nsIDOMDocumentStyle obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMDocumentStyle).GUID, obj);
		return proxyForObject as nsIDOMDocumentStyle;
	}
}
