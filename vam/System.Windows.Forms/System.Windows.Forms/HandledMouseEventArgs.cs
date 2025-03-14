namespace System.Windows.Forms;

public class HandledMouseEventArgs : MouseEventArgs
{
	private bool handled;

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

	public HandledMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
		: base(button, clicks, x, y, delta)
	{
		handled = false;
	}

	public HandledMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta, bool defaultHandledValue)
		: base(button, clicks, x, y, delta)
	{
		handled = defaultHandledValue;
	}
}
