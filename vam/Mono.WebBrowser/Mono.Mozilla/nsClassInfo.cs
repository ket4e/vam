using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsClassInfo
{
	public static nsIClassInfo GetProxy(IWebBrowser control, nsIClassInfo obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIClassInfo).GUID, obj);
		return proxyForObject as nsIClassInfo;
	}
}
