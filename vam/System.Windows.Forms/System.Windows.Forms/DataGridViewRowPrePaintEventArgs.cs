using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

public class DataGridViewRowPrePaintEventArgs : HandledEventArgs
{
	private DataGridView dataGridView;

	private Graphics graphics;

	private Rectangle clipBounds;

	private Rectangle rowBounds;

	private int rowIndex;

	private DataGridViewElementStates rowState;

	private string errorText;

	private DataGridViewCellStyle inheritedRowStyle;

	private bool isFirstDisplayedRow;

	private bool isLastVisibleRow;

	private DataGridViewPaintParts paintParts;

	public Rectangle ClipBounds
	{
		get
		{
			return clipBounds;
		}
		set
		{
			clipBounds = value;
		}
	}

	public string ErrorText => errorText;

	public Graphics Graphics => graphics;

	public DataGridViewCellStyle InheritedRowStyle => inheritedRowStyle;

	public bool IsFirstDisplayedRow => isFirstDisplayedRow;

	public bool IsLastVisibleRow => isLastVisibleRow;

	public DataGridViewPaintParts PaintParts
	{
		get
		{
			return paintParts;
		}
		set
		{
			paintParts = value;
		}
	}

	public Rectangle RowBounds => rowBounds;

	public int RowIndex => rowIndex;

	public DataGridViewElementStates State => rowState;

	public DataGridViewRowPrePaintEventArgs(DataGridView dataGridView, Graphics graphics, Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, string errorText, DataGridViewCellStyle inheritedRowStyle, bool isFirstDisplayedRow, bool isLastVisibleRow)
	{
		this.dataGridView = dataGridView;
		this.graphics = graphics;
		this.clipBounds = clipBounds;
		this.rowBounds = rowBounds;
		this.rowIndex = rowIndex;
		this.rowState = rowState;
		this.errorText = errorText;
		this.inheritedRowStyle = inheritedRowStyle;
		this.isFirstDisplayedRow = isFirstDisplayedRow;
		this.isLastVisibleRow = isLastVisibleRow;
	}

	public void DrawFocus(Rectangle bounds, bool cellsPaintSelectionBackground)
	{
		if (rowIndex < 0 || rowIndex >= dataGridView.Rows.Count)
		{
			throw new InvalidOperationException("Invalid RowIndex.");
		}
		DataGridViewRow rowInternal = dataGridView.GetRowInternal(rowIndex);
		rowInternal.PaintCells(graphics, clipBounds, bounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, DataGridViewPaintParts.Focus);
	}

	public void PaintCells(Rectangle clipBounds, DataGridViewPaintParts paintParts)
	{
		if (rowIndex < 0 || rowIndex >= dataGridView.Rows.Count)
		{
			throw new InvalidOperationException("Invalid RowIndex.");
		}
		DataGridViewRow rowInternal = dataGridView.GetRowInternal(rowIndex);
		rowInternal.PaintCells(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
	}

	public void PaintCellsBackground(Rectangle clipBounds, bool cellsPaintSelectionBackground)
	{
		if (cellsPaintSelectionBackground)
		{
			PaintCells(clipBounds, DataGridViewPaintParts.All);
		}
		else
		{
			PaintCells(clipBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border | DataGridViewPaintParts.ContentBackground | DataGridViewPaintParts.ContentForeground | DataGridViewPaintParts.ErrorIcon | DataGridViewPaintParts.Focus);
		}
	}

	public void PaintCellsContent(Rectangle clipBounds)
	{
		PaintCells(clipBounds, DataGridViewPaintParts.ContentBackground | DataGridViewPaintParts.ContentForeground);
	}

	public void PaintHeader(bool paintSelectionBackground)
	{
		if (paintSelectionBackground)
		{
			PaintHeader(DataGridViewPaintParts.All);
		}
		else
		{
			PaintHeader(DataGridViewPaintParts.Background | DataGridViewPaintParts.Border | DataGridViewPaintParts.ContentBackground | DataGridViewPaintParts.ContentForeground | DataGridViewPaintParts.ErrorIcon | DataGridViewPaintParts.Focus);
		}
	}

	public void PaintHeader(DataGridViewPaintParts paintParts)
	{
		if (rowIndex < 0 || rowIndex >= dataGridView.Rows.Count)
		{
			throw new InvalidOperationException("Invalid RowIndex.");
		}
		DataGridViewRow rowInternal = dataGridView.GetRowInternal(rowIndex);
		rowInternal.PaintHeader(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
	}
}
