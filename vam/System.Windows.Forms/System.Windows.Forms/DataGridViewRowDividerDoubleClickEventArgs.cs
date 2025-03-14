namespace System.Windows.Forms;

public class DataGridViewRowDividerDoubleClickEventArgs : HandledMouseEventArgs
{
	private int rowIndex;

	public int RowIndex => rowIndex;

	public DataGridViewRowDividerDoubleClickEventArgs(int rowIndex, HandledMouseEventArgs e)
		: base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
	{
		this.rowIndex = rowIndex;
	}
}
