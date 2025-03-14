using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDocumentEncoder
{
	public static nsIDocumentEncoder GetProxy(IWebBrowser control, nsIDocumentEncoder obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDocumentEncoder).GUID, obj);
		return proxyForObject as nsIDocumentEncoder;
	}
}
