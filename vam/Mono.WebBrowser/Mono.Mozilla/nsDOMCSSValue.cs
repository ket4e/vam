using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMCSSValue
{
	public static nsIDOMCSSValue GetProxy(IWebBrowser control, nsIDOMCSSValue obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMCSSValue).GUID, obj);
		return proxyForObject as nsIDOMCSSValue;
	}
}
