namespace System.Windows.Forms;

public class DataGridViewRowsRemovedEventArgs : EventArgs
{
	private int rowIndex;

	private int rowCount;

	public int RowCount => rowCount;

	public int RowIndex => rowIndex;

	public DataGridViewRowsRemovedEventArgs(int rowIndex, int rowCount)
	{
		this.rowIndex = rowIndex;
		this.rowCount = rowCount;
	}
}
