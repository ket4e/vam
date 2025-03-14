using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsWebProgressListener
{
	public static nsIWebProgressListener GetProxy(IWebBrowser control, nsIWebProgressListener obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIWebProgressListener).GUID, obj);
		return proxyForObject as nsIWebProgressListener;
	}
}
