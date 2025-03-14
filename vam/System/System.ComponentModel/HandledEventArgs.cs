namespace System.ComponentModel;

public class HandledEventArgs : EventArgs
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

	public HandledEventArgs()
	{
		handled = false;
	}

	public HandledEventArgs(bool defaultHandledValue)
	{
		handled = defaultHandledValue;
	}
}
