using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMCSSStyleDeclaration
{
	public static nsIDOMCSSStyleDeclaration GetProxy(IWebBrowser control, nsIDOMCSSStyleDeclaration obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMCSSStyleDeclaration).GUID, obj);
		return proxyForObject as nsIDOMCSSStyleDeclaration;
	}
}
