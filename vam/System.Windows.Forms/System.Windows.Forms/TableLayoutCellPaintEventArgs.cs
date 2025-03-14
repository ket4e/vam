using System.Drawing;

namespace System.Windows.Forms;

public class TableLayoutCellPaintEventArgs : PaintEventArgs
{
	private Rectangle cell_bounds;

	private int column;

	private int row;

	public Rectangle CellBounds => cell_bounds;

	public int Column => column;

	public int Row => row;

	public TableLayoutCellPaintEventArgs(Graphics g, Rectangle clipRectangle, Rectangle cellBounds, int column, int row)
		: base(g, clipRectangle)
	{
		cell_bounds = cellBounds;
		this.column = column;
		this.row = row;
	}
}
