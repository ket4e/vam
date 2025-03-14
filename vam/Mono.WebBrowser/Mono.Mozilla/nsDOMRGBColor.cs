using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMRGBColor
{
	public static nsIDOMRGBColor GetProxy(IWebBrowser control, nsIDOMRGBColor obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMRGBColor).GUID, obj);
		return proxyForObject as nsIDOMRGBColor;
	}
}
