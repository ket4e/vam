using System.ComponentModel;

namespace System.Windows.Forms;

public class DataGridViewCellCancelEventArgs : CancelEventArgs
{
	private int columnIndex;

	private int rowIndex;

	public int ColumnIndex => columnIndex;

	public int RowIndex => rowIndex;

	public DataGridViewCellCancelEventArgs(int columnIndex, int rowIndex)
	{
		this.columnIndex = columnIndex;
		this.rowIndex = rowIndex;
	}
}
