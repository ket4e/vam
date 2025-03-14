namespace System.Windows.Forms;

public class WebBrowserDocumentCompletedEventArgs : EventArgs
{
	private Uri url;

	public Uri Url => url;

	public WebBrowserDocumentCompletedEventArgs(Uri url)
	{
		this.url = url;
	}
}
