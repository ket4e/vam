using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMDocumentFragment
{
	public static nsIDOMDocumentFragment GetProxy(IWebBrowser control, nsIDOMDocumentFragment obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMDocumentFragment).GUID, obj);
		return proxyForObject as nsIDOMDocumentFragment;
	}
}
