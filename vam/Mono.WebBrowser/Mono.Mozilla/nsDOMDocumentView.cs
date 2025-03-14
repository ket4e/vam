using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMDocumentView
{
	public static nsIDOMDocumentView GetProxy(IWebBrowser control, nsIDOMDocumentView obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMDocumentView).GUID, obj);
		return proxyForObject as nsIDOMDocumentView;
	}
}
