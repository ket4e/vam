using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMDOMImplementation
{
	public static nsIDOMDOMImplementation GetProxy(IWebBrowser control, nsIDOMDOMImplementation obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMDOMImplementation).GUID, obj);
		return proxyForObject as nsIDOMDOMImplementation;
	}
}
