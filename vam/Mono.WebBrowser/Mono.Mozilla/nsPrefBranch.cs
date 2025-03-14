using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsPrefBranch
{
	public static nsIPrefBranch GetProxy(IWebBrowser control, nsIPrefBranch obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIPrefBranch).GUID, obj);
		return proxyForObject as nsIPrefBranch;
	}
}
