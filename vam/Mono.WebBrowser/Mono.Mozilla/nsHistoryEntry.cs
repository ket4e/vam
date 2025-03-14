using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsHistoryEntry
{
	public static nsIHistoryEntry GetProxy(IWebBrowser control, nsIHistoryEntry obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIHistoryEntry).GUID, obj);
		return proxyForObject as nsIHistoryEntry;
	}
}
