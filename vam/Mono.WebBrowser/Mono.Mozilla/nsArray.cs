using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsArray
{
	public static nsIArray GetProxy(IWebBrowser control, nsIArray obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIArray).GUID, obj);
		return proxyForObject as nsIArray;
	}
}
