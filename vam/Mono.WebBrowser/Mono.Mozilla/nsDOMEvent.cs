using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMEvent
{
	public static nsIDOMEvent GetProxy(IWebBrowser control, nsIDOMEvent obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMEvent).GUID, obj);
		return proxyForObject as nsIDOMEvent;
	}
}
