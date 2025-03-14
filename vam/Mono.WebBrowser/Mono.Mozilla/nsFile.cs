using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsFile
{
	public static nsIFile GetProxy(IWebBrowser control, nsIFile obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIFile).GUID, obj);
		return proxyForObject as nsIFile;
	}
}
