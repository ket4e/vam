using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsChannel
{
	public static nsIChannel GetProxy(IWebBrowser control, nsIChannel obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIChannel).GUID, obj);
		return proxyForObject as nsIChannel;
	}
}
