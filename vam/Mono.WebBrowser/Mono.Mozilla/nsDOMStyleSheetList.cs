using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMStyleSheetList
{
	public static nsIDOMStyleSheetList GetProxy(IWebBrowser control, nsIDOMStyleSheetList obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMStyleSheetList).GUID, obj);
		return proxyForObject as nsIDOMStyleSheetList;
	}
}
