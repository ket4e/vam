using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMUIEvent
{
	public static nsIDOMUIEvent GetProxy(IWebBrowser control, nsIDOMUIEvent obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMUIEvent).GUID, obj);
		return proxyForObject as nsIDOMUIEvent;
	}
}
