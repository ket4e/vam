namespace System.Windows.Forms;

public class DataGridViewColumnDividerDoubleClickEventArgs : HandledMouseEventArgs
{
	private int columnIndex;

	public int ColumnIndex => columnIndex;

	public DataGridViewColumnDividerDoubleClickEventArgs(int columnIndex, HandledMouseEventArgs e)
		: base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
	{
		this.columnIndex = columnIndex;
	}
}
