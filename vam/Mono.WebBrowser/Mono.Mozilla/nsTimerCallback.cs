using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsTimerCallback
{
	public static nsITimerCallback GetProxy(IWebBrowser control, nsITimerCallback obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsITimerCallback).GUID, obj);
		return proxyForObject as nsITimerCallback;
	}
}
