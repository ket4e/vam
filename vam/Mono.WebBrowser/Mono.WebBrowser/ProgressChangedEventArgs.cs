using System;

namespace Mono.WebBrowser;

public class ProgressChangedEventArgs : EventArgs
{
	private int progress;

	private int maxProgress;

	public int Progress => progress;

	public int MaxProgress => maxProgress;

	public ProgressChangedEventArgs(int progress, int maxProgress)
	{
		this.progress = progress;
		this.maxProgress = maxProgress;
	}
}
