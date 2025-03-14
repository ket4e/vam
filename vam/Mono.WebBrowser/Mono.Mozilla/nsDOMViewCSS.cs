using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMViewCSS
{
	public static nsIDOMViewCSS GetProxy(IWebBrowser control, nsIDOMViewCSS obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMViewCSS).GUID, obj);
		return proxyForObject as nsIDOMViewCSS;
	}
}
