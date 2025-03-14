namespace System.Windows.Forms;

public class DataGridViewCellContextMenuStripNeededEventArgs : DataGridViewCellEventArgs
{
	private ContextMenuStrip contextMenuStrip;

	public ContextMenuStrip ContextMenuStrip
	{
		get
		{
			return contextMenuStrip;
		}
		set
		{
			contextMenuStrip = value;
		}
	}

	public DataGridViewCellContextMenuStripNeededEventArgs(int columnIndex, int rowIndex)
		: base(columnIndex, rowIndex)
	{
	}
}
