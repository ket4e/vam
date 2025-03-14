namespace ZenFulcrum.EmbeddedBrowser;

public static class ErrorGenerator
{
	private const string htmlIntro = "<!DOCTYPE html><html><head><meta charset=\"UTF-8\">\r\n<style type='text/css'>\r\n\tbody { background: rgba(255, 255, 255, .7); color: black; }\r\n\t#loadError .detail { font-size: 90%; color: #444; }\r\n</style></head><body>\r\n";

	private const string htmlOutro = "</body></html>";

	private const string loadTemplate = "<div id='loadError'>\r\n\t<h1>Load Error</h1>\r\n\t<p class='mainError'>{mainError}</p>\r\n\t<p class='detail'>{detail}</p>\r\n</div>";

	private const string sadTemplate = "<div id='sadTab'>\r\n\t<h1>Page Crashed</h1>\r\n\t<p>The renderer process for this page is no longer running.</p>\r\n</div>";

	private static string Encode(string text)
	{
		return text.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;")
			.Replace(">", "&gt;");
	}

	public static string GenerateFetchError(JSONNode err)
	{
		return "<!DOCTYPE html><html><head><meta charset=\"UTF-8\">\r\n<style type='text/css'>\r\n\tbody { background: rgba(255, 255, 255, .7); color: black; }\r\n\t#loadError .detail { font-size: 90%; color: #444; }\r\n</style></head><body>\r\n" + "<div id='loadError'>\r\n\t<h1>Load Error</h1>\r\n\t<p class='mainError'>{mainError}</p>\r\n\t<p class='detail'>{detail}</p>\r\n</div>".Replace("{mainError}", Encode("Failed to load " + err["url"])).Replace("{detail}", Encode(err["error"])) + "</body></html>";
	}

	public static string GenerateCertError(JSONNode err)
	{
		return "<!DOCTYPE html><html><head><meta charset=\"UTF-8\">\r\n<style type='text/css'>\r\n\tbody { background: rgba(255, 255, 255, .7); color: black; }\r\n\t#loadError .detail { font-size: 90%; color: #444; }\r\n</style></head><body>\r\n" + "<div id='loadError'>\r\n\t<h1>Load Error</h1>\r\n\t<p class='mainError'>{mainError}</p>\r\n\t<p class='detail'>{detail}</p>\r\n</div>".Replace("{mainError}", Encode("Failed to load " + err["url"])).Replace("{detail}", Encode(err["error"])) + "</body></html>";
	}

	public static string GenerateSadTabError()
	{
		return "<!DOCTYPE html><html><head><meta charset=\"UTF-8\">\r\n<style type='text/css'>\r\n\tbody { background: rgba(255, 255, 255, .7); color: black; }\r\n\t#loadError .detail { font-size: 90%; color: #444; }\r\n</style></head><body>\r\n<div id='sadTab'>\r\n\t<h1>Page Crashed</h1>\r\n\t<p>The renderer process for this page is no longer running.</p>\r\n</div></body></html>";
	}
}
