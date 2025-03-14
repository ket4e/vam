using System.Drawing;

namespace System.Windows.Forms;

public class DrawListViewColumnHeaderEventArgs : EventArgs
{
	private Color backColor;

	private Rectangle bounds;

	private int columnIndex;

	private bool drawDefault;

	private Font font;

	private Color foreColor;

	private Graphics graphics;

	private ColumnHeader header;

	private ListViewItemStates state;

	public Color BackColor => backColor;

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

	public Font Font => font;

	public Color ForeColor => foreColor;

	public Graphics Graphics => graphics;

	public ColumnHeader Header => header;

	public ListViewItemStates State => state;

	public DrawListViewColumnHeaderEventArgs(Graphics graphics, Rectangle bounds, int columnIndex, ColumnHeader header, ListViewItemStates state, Color foreColor, Color backColor, Font font)
	{
		this.backColor = backColor;
		this.bounds = bounds;
		this.columnIndex = columnIndex;
		this.font = font;
		this.foreColor = foreColor;
		this.graphics = graphics;
		this.header = header;
		this.state = state;
	}

	public void DrawBackground()
	{
		ThemeEngine.Current.CPDrawButton(graphics, bounds, ButtonState.Normal);
	}

	public void DrawText()
	{
		DrawText(TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
	}

	public void DrawText(TextFormatFlags flags)
	{
		TextRenderer.DrawText(bounds: new Rectangle(bounds.X + 8, bounds.Y, bounds.Width - 13, bounds.Height), dc: graphics, text: header.Text, font: font, foreColor: foreColor, flags: flags);
	}
}
