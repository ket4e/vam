using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMCDATASection
{
	public static nsIDOMCDATASection GetProxy(IWebBrowser control, nsIDOMCDATASection obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMCDATASection).GUID, obj);
		return proxyForObject as nsIDOMCDATASection;
	}
}
