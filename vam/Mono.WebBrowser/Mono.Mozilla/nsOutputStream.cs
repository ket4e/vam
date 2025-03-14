using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsOutputStream
{
	public static nsIOutputStream GetProxy(IWebBrowser control, nsIOutputStream obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIOutputStream).GUID, obj);
		return proxyForObject as nsIOutputStream;
	}
}
