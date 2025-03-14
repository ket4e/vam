using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMDocumentType
{
	public static nsIDOMDocumentType GetProxy(IWebBrowser control, nsIDOMDocumentType obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMDocumentType).GUID, obj);
		return proxyForObject as nsIDOMDocumentType;
	}
}
