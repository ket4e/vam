namespace System.Windows.Forms;

public class WebBrowserProgressChangedEventArgs : EventArgs
{
	private long current_progress;

	private long maximum_progress;

	public long CurrentProgress => current_progress;

	public long MaximumProgress => maximum_progress;

	public WebBrowserProgressChangedEventArgs(long currentProgress, long maximumProgress)
	{
		current_progress = currentProgress;
		maximum_progress = maximumProgress;
	}
}
