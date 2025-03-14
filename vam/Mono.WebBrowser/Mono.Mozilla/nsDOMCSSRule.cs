using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMCSSRule
{
	public static nsIDOMCSSRule GetProxy(IWebBrowser control, nsIDOMCSSRule obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMCSSRule).GUID, obj);
		return proxyForObject as nsIDOMCSSRule;
	}
}
