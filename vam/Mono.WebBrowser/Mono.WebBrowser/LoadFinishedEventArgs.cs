using System;

namespace Mono.WebBrowser;

public class LoadFinishedEventArgs : EventArgs
{
	private string uri;

	public string Uri => uri;

	public LoadFinishedEventArgs(string uri)
	{
		this.uri = uri;
	}
}
