using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMDOMStringList
{
	public static nsIDOMDOMStringList GetProxy(IWebBrowser control, nsIDOMDOMStringList obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMDOMStringList).GUID, obj);
		return proxyForObject as nsIDOMDOMStringList;
	}
}
