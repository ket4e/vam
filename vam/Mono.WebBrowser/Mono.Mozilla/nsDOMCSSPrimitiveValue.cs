using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMCSSPrimitiveValue
{
	public static nsIDOMCSSPrimitiveValue GetProxy(IWebBrowser control, nsIDOMCSSPrimitiveValue obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMCSSPrimitiveValue).GUID, obj);
		return proxyForObject as nsIDOMCSSPrimitiveValue;
	}
}
