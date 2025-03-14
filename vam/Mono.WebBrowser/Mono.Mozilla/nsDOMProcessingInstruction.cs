using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class nsDOMProcessingInstruction
{
	public static nsIDOMProcessingInstruction GetProxy(IWebBrowser control, nsIDOMProcessingInstruction obj)
	{
		object proxyForObject = Base.GetProxyForObject(control, typeof(nsIDOMProcessingInstruction).GUID, obj);
		return proxyForObject as nsIDOMProcessingInstruction;
	}
}
