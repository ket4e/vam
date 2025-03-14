using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsWeakReference
{
	public static nsIWeakReference GetProxy(IWebBrowser control, nsIWeakReference obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIWeakReference).GUID, obj);
		return proxyForObject as nsIWeakReference;
	}
}
