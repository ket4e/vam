using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMText
{
	public static nsIDOMText GetProxy(IWebBrowser control, nsIDOMText obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMText).GUID, obj);
		return proxyForObject as nsIDOMText;
	}
}
