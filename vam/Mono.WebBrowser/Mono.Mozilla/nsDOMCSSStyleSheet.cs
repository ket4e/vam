using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMCSSStyleSheet
{
	public static nsIDOMCSSStyleSheet GetProxy(IWebBrowser control, nsIDOMCSSStyleSheet obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMCSSStyleSheet).GUID, obj);
		return proxyForObject as nsIDOMCSSStyleSheet;
	}
}
