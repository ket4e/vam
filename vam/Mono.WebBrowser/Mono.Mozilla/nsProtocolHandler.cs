using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsProtocolHandler
{
	public static nsIProtocolHandler GetProxy(IWebBrowser control, nsIProtocolHandler obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIProtocolHandler).GUID, obj);
		return proxyForObject as nsIProtocolHandler;
	}
}
