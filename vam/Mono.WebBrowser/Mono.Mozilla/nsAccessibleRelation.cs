using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsAccessibleRelation
{
	public static nsIAccessibleRelation GetProxy(IWebBrowser control, nsIAccessibleRelation obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIAccessibleRelation).GUID, obj);
		return proxyForObject as nsIAccessibleRelation;
	}
}
