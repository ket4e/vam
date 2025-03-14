using System.ComponentModel;

namespace System.Windows.Forms;

public class ColumnReorderedEventArgs : CancelEventArgs
{
	private ColumnHeader header;

	private int new_display_index;

	private int old_display_index;

	public int OldDisplayIndex => old_display_index;

	public int NewDisplayIndex => new_display_index;

	public ColumnHeader Header => header;

	public ColumnReorderedEventArgs(int oldDisplayIndex, int newDisplayIndex, ColumnHeader header)
	{
		old_display_index = oldDisplayIndex;
		new_display_index = newDisplayIndex;
		this.header = header;
	}
}
