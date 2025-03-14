using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMStyleSheet
{
	public static nsIDOMStyleSheet GetProxy(IWebBrowser control, nsIDOMStyleSheet obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMStyleSheet).GUID, obj);
		return proxyForObject as nsIDOMStyleSheet;
	}
}
