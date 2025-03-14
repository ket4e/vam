using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMNodeList
{
	public static nsIDOMNodeList GetProxy(IWebBrowser control, nsIDOMNodeList obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMNodeList).GUID, obj);
		return proxyForObject as nsIDOMNodeList;
	}
}
