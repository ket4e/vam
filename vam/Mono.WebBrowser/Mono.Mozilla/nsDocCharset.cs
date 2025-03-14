using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDocCharset
{
	public static nsIDocCharset GetProxy(IWebBrowser control, nsIDocCharset obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDocCharset).GUID, obj);
		return proxyForObject as nsIDocCharset;
	}
}
