using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMDocumentRange
{
	public static nsIDOMDocumentRange GetProxy(IWebBrowser control, nsIDOMDocumentRange obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMDocumentRange).GUID, obj);
		return proxyForObject as nsIDOMDocumentRange;
	}
}
