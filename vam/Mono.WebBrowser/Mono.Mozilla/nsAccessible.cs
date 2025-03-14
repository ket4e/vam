using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsAccessible
{
	public static nsIAccessible GetProxy(IWebBrowser control, nsIAccessible obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIAccessible).GUID, obj);
		return proxyForObject as nsIAccessible;
	}
}
