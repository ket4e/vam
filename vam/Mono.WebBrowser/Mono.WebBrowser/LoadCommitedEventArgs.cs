using System;

namespace Mono.WebBrowser;

public class LoadCommitedEventArgs : EventArgs
{
	private string uri;

	public string Uri => uri;

	public LoadCommitedEventArgs(string uri)
	{
		this.uri = uri;
	}
}
