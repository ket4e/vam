using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMAttr
{
	public static nsIDOMAttr GetProxy(IWebBrowser control, nsIDOMAttr obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMAttr).GUID, obj);
		return proxyForObject as nsIDOMAttr;
	}
}
