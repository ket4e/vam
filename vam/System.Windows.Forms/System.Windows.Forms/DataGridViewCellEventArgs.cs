namespace System.Windows.Forms;

public class DataGridViewCellEventArgs : EventArgs
{
	private int columnIndex;

	private int rowIndex;

	public int ColumnIndex => columnIndex;

	public int RowIndex => rowIndex;

	public DataGridViewCellEventArgs(int columnIndex, int rowIndex)
	{
		this.columnIndex = columnIndex;
		this.rowIndex = rowIndex;
	}
}
