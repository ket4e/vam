using System.ComponentModel;

namespace System.Windows.Forms;

public class WebBrowserNavigatingEventArgs : CancelEventArgs
{
	private Uri url;

	private string target_frame_name;

	public Uri Url => url;

	public string TargetFrameName => target_frame_name;

	public WebBrowserNavigatingEventArgs(Uri url, string targetFrameName)
	{
		this.url = url;
		target_frame_name = targetFrameName;
	}
}
