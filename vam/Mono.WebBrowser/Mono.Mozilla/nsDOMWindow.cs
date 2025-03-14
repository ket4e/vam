using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMWindow
{
	public static nsIDOMWindow GetProxy(IWebBrowser control, nsIDOMWindow obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMWindow).GUID, obj);
		return proxyForObject as nsIDOMWindow;
	}
}
