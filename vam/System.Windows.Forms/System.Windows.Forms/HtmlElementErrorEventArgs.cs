namespace System.Windows.Forms;

public sealed class HtmlElementErrorEventArgs : EventArgs
{
	private string description;

	private bool handled;

	private int line_number;

	private Uri url;

	public string Description => description;

	public bool Handled
	{
		get
		{
			return handled;
		}
		set
		{
			handled = value;
		}
	}

	public int LineNumber => line_number;

	public Uri Url => url;

	internal HtmlElementErrorEventArgs(string description, int lineNumber, Uri url)
	{
		this.description = description;
		line_number = lineNumber;
		this.url = url;
	}
}
