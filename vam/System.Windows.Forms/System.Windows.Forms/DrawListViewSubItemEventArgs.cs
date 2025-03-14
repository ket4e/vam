using System.Drawing;

namespace System.Windows.Forms;

public class DrawListViewSubItemEventArgs : EventArgs
{
	private Rectangle bounds;

	private int columnIndex;

	private bool drawDefault;

	private Graphics graphics;

	private ColumnHeader header;

	private ListViewItem item;

	private int itemIndex;

	private ListViewItemStates itemState;

	private ListViewItem.ListViewSubItem subItem;

	public Rectangle Bounds => bounds;

	public int ColumnIndex => columnIndex;

	public bool DrawDefault
	{
		get
		{
			return drawDefault;
		}
		set
		{
			drawDefault = value;
		}
	}

	public Graphics Graphics => graphics;

	public ColumnHeader Header => header;

	public ListViewItem Item => item;

	public int ItemIndex => itemIndex;

	public ListViewItemStates ItemState => itemState;

	public ListViewItem.ListViewSubItem SubItem => subItem;

	public DrawListViewSubItemEventArgs(Graphics graphics, Rectangle bounds, ListViewItem item, ListViewItem.ListViewSubItem subItem, int itemIndex, int columnIndex, ColumnHeader header, ListViewItemStates itemState)
	{
		this.bounds = bounds;
		this.columnIndex = columnIndex;
		this.graphics = graphics;
		this.header = header;
		this.item = item;
		this.itemIndex = itemIndex;
		this.itemState = itemState;
		this.subItem = subItem;
	}

	public void DrawBackground()
	{
		graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(subItem.BackColor), bounds);
	}

	public void DrawFocusRectangle(Rectangle bounds)
	{
		if ((itemState & ListViewItemStates.Focused) != 0)
		{
			Rectangle rectangle = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 1, bounds.Height - 1);
			ThemeEngine.Current.CPDrawFocusRectangle(graphics, rectangle, subItem.ForeColor, subItem.BackColor);
		}
	}

	public void DrawText()
	{
		DrawText(TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis);
	}

	public void DrawText(TextFormatFlags flags)
	{
		TextRenderer.DrawText(bounds: new Rectangle(bounds.X + 8, bounds.Y, bounds.Width - 13, bounds.Height), dc: graphics, text: subItem.Text, font: subItem.Font, foreColor: subItem.ForeColor, flags: flags);
	}
}
