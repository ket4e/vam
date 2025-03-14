namespace System.Windows.Forms;

public class DataGridViewCellParsingEventArgs : ConvertEventArgs
{
	private int columnIndex;

	private DataGridViewCellStyle inheritedCellStyle;

	private bool parsingApplied;

	private int rowIndex;

	public DataGridViewCellStyle InheritedCellStyle
	{
		get
		{
			return inheritedCellStyle;
		}
		set
		{
			inheritedCellStyle = value;
		}
	}

	public int ColumnIndex => columnIndex;

	public bool ParsingApplied
	{
		get
		{
			return parsingApplied;
		}
		set
		{
			parsingApplied = value;
		}
	}

	public int RowIndex => rowIndex;

	public DataGridViewCellParsingEventArgs(int rowIndex, int columnIndex, object value, Type desiredType, DataGridViewCellStyle inheritedCellStyle)
		: base(value, desiredType)
	{
		this.columnIndex = columnIndex;
		this.rowIndex = rowIndex;
		this.inheritedCellStyle = inheritedCellStyle;
	}
}
