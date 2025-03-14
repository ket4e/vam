using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsRequest
{
	public static nsIRequest GetProxy(IWebBrowser control, nsIRequest obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIRequest).GUID, obj);
		return proxyForObject as nsIRequest;
	}
}
