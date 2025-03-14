using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMComment
{
	public static nsIDOMComment GetProxy(IWebBrowser control, nsIDOMComment obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMComment).GUID, obj);
		return proxyForObject as nsIDOMComment;
	}
}
