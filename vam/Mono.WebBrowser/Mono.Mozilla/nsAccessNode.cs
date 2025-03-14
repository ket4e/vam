using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsAccessNode
{
	public static nsIAccessNode GetProxy(IWebBrowser control, nsIAccessNode obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIAccessNode).GUID, obj);
		return proxyForObject as nsIAccessNode;
	}
}
