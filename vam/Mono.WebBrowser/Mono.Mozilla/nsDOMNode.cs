using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMNode
{
	public static nsIDOMNode GetProxy(IWebBrowser control, nsIDOMNode obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMNode).GUID, obj);
		return proxyForObject as nsIDOMNode;
	}
}
