using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsSimpleEnumerator
{
	public static nsISimpleEnumerator GetProxy(IWebBrowser control, nsISimpleEnumerator obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsISimpleEnumerator).GUID, obj);
		return proxyForObject as nsISimpleEnumerator;
	}
}
