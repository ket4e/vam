using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsInterfaceRequestor
{
	public static nsIInterfaceRequestor GetProxy(IWebBrowser control, nsIInterfaceRequestor obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIInterfaceRequestor).GUID, obj);
		return proxyForObject as nsIInterfaceRequestor;
	}
}
