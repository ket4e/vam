namespace System.Windows.Forms;

public class DataGridViewCellFormattingEventArgs : ConvertEventArgs
{
	private int columnIndex;

	private DataGridViewCellStyle cellStyle;

	private bool formattingApplied;

	private int rowIndex;

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

	public int ColumnIndex => columnIndex;

	public bool FormattingApplied
	{
		get
		{
			return formattingApplied;
		}
		set
		{
			formattingApplied = value;
		}
	}

	public int RowIndex => rowIndex;

	public DataGridViewCellFormattingEventArgs(int columnIndex, int rowIndex, object value, Type desiredType, DataGridViewCellStyle cellStyle)
		: base(value, desiredType)
	{
		this.columnIndex = columnIndex;
		this.rowIndex = rowIndex;
		this.cellStyle = cellStyle;
	}
}
