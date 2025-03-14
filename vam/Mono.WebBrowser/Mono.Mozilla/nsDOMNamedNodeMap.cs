using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMNamedNodeMap
{
	public static nsIDOMNamedNodeMap GetProxy(IWebBrowser control, nsIDOMNamedNodeMap obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMNamedNodeMap).GUID, obj);
		return proxyForObject as nsIDOMNamedNodeMap;
	}
}
