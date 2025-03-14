using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

public class DataGridViewCellPaintingEventArgs : HandledEventArgs
{
	private DataGridView dataGridView;

	private Graphics graphics;

	private Rectangle clipBounds;

	private Rectangle cellBounds;

	private int rowIndex;

	private int columnIndex;

	private DataGridViewElementStates cellState;

	private object cellValue;

	private object formattedValue;

	private string errorText;

	private DataGridViewCellStyle cellStyle;

	private DataGridViewAdvancedBorderStyle advancedBorderStyle;

	private DataGridViewPaintParts paintParts;

	public DataGridViewAdvancedBorderStyle AdvancedBorderStyle => advancedBorderStyle;

	public Rectangle CellBounds => cellBounds;

	public DataGridViewCellStyle CellStyle => cellStyle;

	public Rectangle ClipBounds => clipBounds;

	public int ColumnIndex => columnIndex;

	public string ErrorText => errorText;

	public object FormattedValue => formattedValue;

	public Graphics Graphics => graphics;

	public DataGridViewPaintParts PaintParts => paintParts;

	public int RowIndex => rowIndex;

	public DataGridViewElementStates State => cellState;

	public object Value => cellValue;

	public DataGridViewCellPaintingEventArgs(DataGridView dataGridView, Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, int columnIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
	{
		this.dataGridView = dataGridView;
		this.graphics = graphics;
		this.clipBounds = clipBounds;
		this.cellBounds = cellBounds;
		this.rowIndex = rowIndex;
		this.columnIndex = columnIndex;
		this.cellState = cellState;
		cellValue = value;
		this.formattedValue = formattedValue;
		this.errorText = errorText;
		this.cellStyle = cellStyle;
		this.advancedBorderStyle = advancedBorderStyle;
		this.paintParts = paintParts;
	}

	public void Paint(Rectangle clipBounds, DataGridViewPaintParts paintParts)
	{
		if (rowIndex < -1 || rowIndex >= dataGridView.Rows.Count)
		{
			throw new InvalidOperationException("Invalid \"RowIndex.\"");
		}
		if (columnIndex < -1 || columnIndex >= dataGridView.Columns.Count)
		{
			throw new InvalidOperationException("Invalid \"ColumnIndex.\"");
		}
		DataGridViewCell dataGridViewCell = ((rowIndex == -1 && columnIndex == -1) ? dataGridView.TopLeftHeaderCell : ((rowIndex == -1) ? dataGridView.Columns[columnIndex].HeaderCell : ((columnIndex != -1) ? dataGridView.Rows[rowIndex].Cells[columnIndex] : dataGridView.Rows[rowIndex].HeaderCell)));
		dataGridViewCell.PaintInternal(graphics, clipBounds, cellBounds, rowIndex, cellState, Value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
	}

	public void PaintBackground(Rectangle clipBounds, bool cellsPaintSelectionBackground)
	{
		Paint(clipBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);
	}

	[System.MonoInternalNote("Needs row header cell edit pencil glyph")]
	public void PaintContent(Rectangle clipBounds)
	{
		Paint(clipBounds, DataGridViewPaintParts.ContentBackground | DataGridViewPaintParts.ContentForeground);
	}
}
