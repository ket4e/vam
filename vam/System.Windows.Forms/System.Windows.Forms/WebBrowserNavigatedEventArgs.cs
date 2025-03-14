namespace System.Windows.Forms;

public class WebBrowserNavigatedEventArgs : EventArgs
{
	private Uri url;

	public Uri Url => url;

	public WebBrowserNavigatedEventArgs(Uri url)
	{
		this.url = url;
	}
}
