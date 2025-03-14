using System.ComponentModel;

namespace System.Windows.Forms;

public class ColumnWidthChangingEventArgs : CancelEventArgs
{
	private int column_index;

	private int new_width;

	public int ColumnIndex => column_index;

	public int NewWidth
	{
		get
		{
			return new_width;
		}
		set
		{
			new_width = value;
		}
	}

	public ColumnWidthChangingEventArgs(int columnIndex, int newWidth)
		: this(columnIndex, newWidth, cancel: false)
	{
	}

	public ColumnWidthChangingEventArgs(int columnIndex, int newWidth, bool cancel)
		: base(cancel)
	{
		column_index = columnIndex;
		new_width = newWidth;
	}
}
