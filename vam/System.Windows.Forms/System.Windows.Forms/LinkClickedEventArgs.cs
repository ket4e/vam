using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public class LinkClickedEventArgs : EventArgs
{
	private string link_text;

	public string LinkText => link_text;

	public LinkClickedEventArgs(string linkText)
	{
		link_text = linkText;
	}
}
