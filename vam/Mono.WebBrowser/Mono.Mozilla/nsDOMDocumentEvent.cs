using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMDocumentEvent
{
	public static nsIDOMDocumentEvent GetProxy(IWebBrowser control, nsIDOMDocumentEvent obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMDocumentEvent).GUID, obj);
		return proxyForObject as nsIDOMDocumentEvent;
	}
}
