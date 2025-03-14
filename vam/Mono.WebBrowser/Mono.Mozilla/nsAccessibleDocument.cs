using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsAccessibleDocument
{
	public static nsIAccessibleDocument GetProxy(IWebBrowser control, nsIAccessibleDocument obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIAccessibleDocument).GUID, obj);
		return proxyForObject as nsIAccessibleDocument;
	}
}
