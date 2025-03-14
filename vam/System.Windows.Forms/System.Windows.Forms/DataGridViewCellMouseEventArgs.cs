namespace System.Windows.Forms;

public class DataGridViewCellMouseEventArgs : MouseEventArgs
{
	private int columnIndex;

	private int rowIndex;

	public int ColumnIndex => columnIndex;

	public int RowIndex => rowIndex;

	public DataGridViewCellMouseEventArgs(int columnIndex, int rowIndex, int localX, int localY, MouseEventArgs e)
		: base(e.Button, e.Clicks, localX, localY, e.Delta)
	{
		this.columnIndex = columnIndex;
		this.rowIndex = rowIndex;
	}
}
