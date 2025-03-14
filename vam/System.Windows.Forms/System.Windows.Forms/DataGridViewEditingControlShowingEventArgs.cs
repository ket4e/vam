namespace System.Windows.Forms;

public class DataGridViewEditingControlShowingEventArgs : EventArgs
{
	private Control control;

	private DataGridViewCellStyle cellStyle;

	public DataGridViewCellStyle CellStyle
	{
		get
		{
			return cellStyle;
		}
		set
		{
			cellStyle = value;
		}
	}

	public Control Control => control;

	public DataGridViewEditingControlShowingEventArgs(Control control, DataGridViewCellStyle cellStyle)
	{
		this.control = control;
		this.cellStyle = cellStyle;
	}
}
