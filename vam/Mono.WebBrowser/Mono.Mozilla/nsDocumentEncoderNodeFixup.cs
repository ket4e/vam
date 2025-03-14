using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDocumentEncoderNodeFixup
{
	public static nsIDocumentEncoderNodeFixup GetProxy(IWebBrowser control, nsIDocumentEncoderNodeFixup obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDocumentEncoderNodeFixup).GUID, obj);
		return proxyForObject as nsIDocumentEncoderNodeFixup;
	}
}
