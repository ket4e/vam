using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMEventTarget
{
	public static nsIDOMEventTarget GetProxy(IWebBrowser control, nsIDOMEventTarget obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMEventTarget).GUID, obj);
		return proxyForObject as nsIDOMEventTarget;
	}
}
