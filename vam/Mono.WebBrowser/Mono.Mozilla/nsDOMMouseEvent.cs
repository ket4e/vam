using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMMouseEvent
{
	public static nsIDOMMouseEvent GetProxy(IWebBrowser control, nsIDOMMouseEvent obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMMouseEvent).GUID, obj);
		return proxyForObject as nsIDOMMouseEvent;
	}
}
