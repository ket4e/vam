using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsAccessibilityService
{
	public static nsIAccessibilityService GetProxy(IWebBrowser control, nsIAccessibilityService obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIAccessibilityService).GUID, obj);
		return proxyForObject as nsIAccessibilityService;
	}
}
