namespace System.Windows.Forms;

public class DataGridViewRowsAddedEventArgs : EventArgs
{
	private int rowIndex;

	private int rowCount;

	public int RowCount => rowCount;

	public int RowIndex => rowIndex;

	public DataGridViewRowsAddedEventArgs(int rowIndex, int rowCount)
	{
		this.rowIndex = rowIndex;
		this.rowCount = rowCount;
	}
}
