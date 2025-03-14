using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsPersistentProperties
{
	public static nsIPersistentProperties GetProxy(IWebBrowser control, nsIPersistentProperties obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIPersistentProperties).GUID, obj);
		return proxyForObject as nsIPersistentProperties;
	}
}
