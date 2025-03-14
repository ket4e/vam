namespace System.Windows.Forms;

public class ColumnWidthChangedEventArgs : EventArgs
{
	private int column_index;

	public int ColumnIndex => column_index;

	public ColumnWidthChangedEventArgs(int columnIndex)
	{
		column_index = columnIndex;
	}
}
