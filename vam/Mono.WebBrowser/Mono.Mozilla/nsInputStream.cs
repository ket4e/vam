using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsInputStream
{
	public static nsIInputStream GetProxy(IWebBrowser control, nsIInputStream obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIInputStream).GUID, obj);
		return proxyForObject as nsIInputStream;
	}
}
