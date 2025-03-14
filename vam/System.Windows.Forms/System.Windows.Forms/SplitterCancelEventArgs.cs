using System.ComponentModel;

namespace System.Windows.Forms;

public class SplitterCancelEventArgs : CancelEventArgs
{
	private int mouse_cursor_x;

	private int mouse_cursor_y;

	private int split_x;

	private int split_y;

	public int MouseCursorX => mouse_cursor_x;

	public int MouseCursorY => mouse_cursor_y;

	public int SplitX
	{
		get
		{
			return split_x;
		}
		set
		{
			split_x = value;
		}
	}

	public int SplitY
	{
		get
		{
			return split_y;
		}
		set
		{
			split_y = value;
		}
	}

	public SplitterCancelEventArgs(int mouseCursorX, int mouseCursorY, int splitX, int splitY)
	{
		mouse_cursor_x = mouseCursorX;
		mouse_cursor_y = mouseCursorY;
		split_x = splitX;
		split_y = splitY;
	}
}
