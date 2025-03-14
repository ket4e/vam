using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsAccessibleRetrieval
{
	public static nsIAccessibleRetrieval GetProxy(IWebBrowser control, nsIAccessibleRetrieval obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIAccessibleRetrieval).GUID, obj);
		return proxyForObject as nsIAccessibleRetrieval;
	}
}
