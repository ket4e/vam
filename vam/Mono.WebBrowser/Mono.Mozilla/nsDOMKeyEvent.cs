using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMKeyEvent
{
	public static nsIDOMKeyEvent GetProxy(IWebBrowser control, nsIDOMKeyEvent obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMKeyEvent).GUID, obj);
		return proxyForObject as nsIDOMKeyEvent;
	}
}
