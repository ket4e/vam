using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace ZenFulcrum.EmbeddedBrowser;

internal class EditorWebResources : WebResources
{
	protected string basePath;

	private readonly Regex matchDots = new Regex("(^|[/\\\\])\\.[2,]($|[/\\\\])");

	public EditorWebResources()
	{
		basePath = Path.GetDirectoryName(Application.dataPath) + "/BrowserAssets";
	}

	public override void HandleRequest(int id, string url)
	{
		Uri uri = new Uri(url);
		string text = UnityWebRequest.UnEscapeURL(uri.AbsolutePath);
		if (matchDots.IsMatch(text))
		{
			SendError(id, "Invalid path", 400);
			return;
		}
		FileInfo fileInfo = new FileInfo(Application.dataPath + "/../BrowserAssets/" + text);
		if (!fileInfo.Exists)
		{
			SendError(id, "Not found", 404);
		}
		else
		{
			SendFile(id, fileInfo);
		}
	}
}
