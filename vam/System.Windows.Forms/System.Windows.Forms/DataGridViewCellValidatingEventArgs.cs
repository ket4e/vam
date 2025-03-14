using System.ComponentModel;

namespace System.Windows.Forms;

public class DataGridViewCellValidatingEventArgs : CancelEventArgs
{
	private int columnIndex;

	private object formattedValue;

	private int rowIndex;

	public int ColumnIndex => columnIndex;

	public object FormattedValue => formattedValue;

	public int RowIndex => rowIndex;

	internal DataGridViewCellValidatingEventArgs(int columnIndex, int rowIndex, object formattedValue)
	{
		this.columnIndex = columnIndex;
		this.rowIndex = rowIndex;
		this.formattedValue = formattedValue;
	}
}
