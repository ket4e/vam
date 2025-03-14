using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMAbstractView
{
	public static nsIDOMAbstractView GetProxy(IWebBrowser control, nsIDOMAbstractView obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMAbstractView).GUID, obj);
		return proxyForObject as nsIDOMAbstractView;
	}
}
