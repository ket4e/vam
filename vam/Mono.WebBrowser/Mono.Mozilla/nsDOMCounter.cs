using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMCounter
{
	public static nsIDOMCounter GetProxy(IWebBrowser control, nsIDOMCounter obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMCounter).GUID, obj);
		return proxyForObject as nsIDOMCounter;
	}
}
