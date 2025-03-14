using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMDocument
{
	public static nsIDOMDocument GetProxy(IWebBrowser control, nsIDOMDocument obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMDocument).GUID, obj);
		return proxyForObject as nsIDOMDocument;
	}
}
