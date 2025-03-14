using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public class ScrollEventArgs : EventArgs
{
	private ScrollEventType type;

	private int new_value;

	private int old_value;

	private ScrollOrientation scroll_orientation;

	public int NewValue
	{
		get
		{
			return new_value;
		}
		set
		{
			new_value = value;
		}
	}

	public int OldValue => old_value;

	public ScrollOrientation ScrollOrientation => scroll_orientation;

	public ScrollEventType Type => type;

	public ScrollEventArgs(ScrollEventType type, int newValue)
		: this(type, -1, newValue, ScrollOrientation.HorizontalScroll)
	{
	}

	public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue)
		: this(type, oldValue, newValue, ScrollOrientation.HorizontalScroll)
	{
	}

	public ScrollEventArgs(ScrollEventType type, int newValue, ScrollOrientation scroll)
		: this(type, -1, newValue, scroll)
	{
	}

	public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue, ScrollOrientation scroll)
	{
		new_value = newValue;
		old_value = oldValue;
		scroll_orientation = scroll;
		this.type = type;
	}
}
