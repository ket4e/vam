using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsWebNavigation
{
	public static nsIWebNavigation GetProxy(IWebBrowser control, nsIWebNavigation obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIWebNavigation).GUID, obj);
		return proxyForObject as nsIWebNavigation;
	}
}
