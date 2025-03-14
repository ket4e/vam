using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMBarProp
{
	public static nsIDOMBarProp GetProxy(IWebBrowser control, nsIDOMBarProp obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMBarProp).GUID, obj);
		return proxyForObject as nsIDOMBarProp;
	}
}
