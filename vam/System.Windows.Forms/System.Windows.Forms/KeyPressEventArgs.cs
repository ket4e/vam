using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public class KeyPressEventArgs : EventArgs
{
	private char key_char;

	private bool event_handled;

	public bool Handled
	{
		get
		{
			return event_handled;
		}
		set
		{
			event_handled = value;
		}
	}

	public char KeyChar
	{
		get
		{
			return key_char;
		}
		set
		{
			key_char = value;
		}
	}

	public KeyPressEventArgs(char keyChar)
	{
		key_char = keyChar;
		event_handled = false;
	}
}
