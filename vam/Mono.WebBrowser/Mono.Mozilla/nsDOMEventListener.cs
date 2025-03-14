using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMEventListener
{
	public static nsIDOMEventListener GetProxy(IWebBrowser control, nsIDOMEventListener obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMEventListener).GUID, obj);
		return proxyForObject as nsIDOMEventListener;
	}
}
