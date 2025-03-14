namespace System.Windows.Forms;

public class DataGridViewRowContextMenuStripNeededEventArgs : EventArgs
{
	private int rowIndex;

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

	public int RowIndex => rowIndex;

	public DataGridViewRowContextMenuStripNeededEventArgs(int rowIndex)
	{
		this.rowIndex = rowIndex;
	}
}
