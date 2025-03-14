namespace System.Windows.Forms;

public class DataGridViewCellStyleContentChangedEventArgs : EventArgs
{
	private DataGridViewCellStyle cellStyle;

	private DataGridViewCellStyleScopes cellStyleScope;

	public DataGridViewCellStyle CellStyle => cellStyle;

	public DataGridViewCellStyleScopes CellStyleScope => cellStyleScope;

	internal DataGridViewCellStyleContentChangedEventArgs(DataGridViewCellStyle cellStyle, DataGridViewCellStyleScopes cellStyleScope)
	{
		this.cellStyle = cellStyle;
		this.cellStyleScope = cellStyleScope;
	}
}
