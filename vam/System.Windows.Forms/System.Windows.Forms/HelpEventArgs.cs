using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public class HelpEventArgs : EventArgs
{
	private Point mouse_position;

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

	public Point MousePos => mouse_position;

	public HelpEventArgs(Point mousePos)
	{
		mouse_position = mousePos;
		event_handled = false;
	}
}
