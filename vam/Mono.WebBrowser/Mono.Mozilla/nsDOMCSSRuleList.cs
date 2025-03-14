using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMCSSRuleList
{
	public static nsIDOMCSSRuleList GetProxy(IWebBrowser control, nsIDOMCSSRuleList obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMCSSRuleList).GUID, obj);
		return proxyForObject as nsIDOMCSSRuleList;
	}
}
