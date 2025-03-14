using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public class KeyEventArgs : EventArgs
{
	private Keys key_data;

	private bool event_handled;

	private bool supress_key_press;

	public virtual bool Alt
	{
		get
		{
			if ((key_data & Keys.Alt) == 0)
			{
				return false;
			}
			return true;
		}
	}

	public bool Control
	{
		get
		{
			if ((key_data & Keys.Control) == 0)
			{
				return false;
			}
			return true;
		}
	}

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

	public Keys KeyCode => key_data & Keys.KeyCode;

	public Keys KeyData => key_data;

	public int KeyValue => Convert.ToInt32(key_data);

	public Keys Modifiers => key_data & Keys.Modifiers;

	public virtual bool Shift
	{
		get
		{
			if ((key_data & Keys.Shift) == 0)
			{
				return false;
			}
			return true;
		}
	}

	public bool SuppressKeyPress
	{
		get
		{
			return supress_key_press;
		}
		set
		{
			supress_key_press = value;
			event_handled = value;
		}
	}

	public KeyEventArgs(Keys keyData)
	{
		key_data = keyData | XplatUI.State.ModifierKeys;
		event_handled = false;
	}
}
