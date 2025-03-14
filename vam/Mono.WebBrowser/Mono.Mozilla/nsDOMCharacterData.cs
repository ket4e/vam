using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMCharacterData
{
	public static nsIDOMCharacterData GetProxy(IWebBrowser control, nsIDOMCharacterData obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMCharacterData).GUID, obj);
		return proxyForObject as nsIDOMCharacterData;
	}
}
