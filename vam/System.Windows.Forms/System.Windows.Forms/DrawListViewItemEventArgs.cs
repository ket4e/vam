using System.Drawing;

namespace System.Windows.Forms;

public class DrawListViewItemEventArgs : EventArgs
{
	private Rectangle bounds;

	private bool drawDefault;

	private Graphics graphics;

	private ListViewItem item;

	private int itemIndex;

	private ListViewItemStates state;

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

	public Rectangle Bounds => bounds;

	public Graphics Graphics => graphics;

	public ListViewItem Item => item;

	public int ItemIndex => itemIndex;

	public ListViewItemStates State => state;

	public DrawListViewItemEventArgs(Graphics graphics, ListViewItem item, Rectangle bounds, int itemIndex, ListViewItemStates state)
	{
		this.graphics = graphics;
		this.item = item;
		this.bounds = bounds;
		this.itemIndex = itemIndex;
		this.state = state;
	}

	public void DrawBackground()
	{
		graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(item.BackColor), bounds);
	}

	public void DrawFocusRectangle()
	{
		if ((state & ListViewItemStates.Focused) != 0)
		{
			ThemeEngine.Current.CPDrawFocusRectangle(graphics, bounds, item.ListView.ForeColor, item.ListView.BackColor);
		}
	}

	public void DrawText()
	{
		DrawText(TextFormatFlags.Left);
	}

	public void DrawText(TextFormatFlags flags)
	{
		TextRenderer.DrawText(graphics, item.Text, item.Font, bounds, item.ForeColor, flags);
	}
}
