namespace System.Windows.Forms;

public class DataGridViewCellValueEventArgs : EventArgs
{
	private int columnIndex;

	private int rowIndex;

	private object cellValue;

	public int ColumnIndex => columnIndex;

	public int RowIndex => rowIndex;

	public object Value
	{
		get
		{
			return cellValue;
		}
		set
		{
			cellValue = value;
		}
	}

	public DataGridViewCellValueEventArgs(int columnIndex, int rowIndex)
	{
		this.columnIndex = columnIndex;
		this.rowIndex = rowIndex;
	}
}
