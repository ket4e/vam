using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMMediaList
{
	public static nsIDOMMediaList GetProxy(IWebBrowser control, nsIDOMMediaList obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMMediaList).GUID, obj);
		return proxyForObject as nsIDOMMediaList;
	}
}
