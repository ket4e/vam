using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsCancelable
{
	public static nsICancelable GetProxy(IWebBrowser control, nsICancelable obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsICancelable).GUID, obj);
		return proxyForObject as nsICancelable;
	}
}
