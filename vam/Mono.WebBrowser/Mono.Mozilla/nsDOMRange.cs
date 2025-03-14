using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMRange
{
	public static nsIDOMRange GetProxy(IWebBrowser control, nsIDOMRange obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMRange).GUID, obj);
		return proxyForObject as nsIDOMRange;
	}
}
