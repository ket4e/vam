using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public class QueryContinueDragEventArgs : EventArgs
{
	internal int key_state;

	internal bool escape_pressed;

	internal DragAction drag_action;

	public DragAction Action
	{
		get
		{
			return drag_action;
		}
		set
		{
			drag_action = value;
		}
	}

	public bool EscapePressed => escape_pressed;

	public int KeyState => key_state;

	public QueryContinueDragEventArgs(int keyState, bool escapePressed, DragAction action)
	{
		key_state = keyState;
		escape_pressed = escapePressed;
		drag_action = action;
	}
}
