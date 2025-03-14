using System.Drawing;

namespace System.Windows.Forms;

public class DrawItemEventArgs : EventArgs
{
	private Graphics graphics;

	private Font font;

	private Rectangle rect;

	private int index;

	private DrawItemState state;

	private Color fore_color;

	private Color back_color;

	public Graphics Graphics => graphics;

	public Font Font => font;

	public Rectangle Bounds => rect;

	public int Index => index;

	public DrawItemState State => state;

	public Color BackColor => back_color;

	public Color ForeColor => fore_color;

	public DrawItemEventArgs(Graphics graphics, Font font, Rectangle rect, int index, DrawItemState state)
		: this(graphics, font, rect, index, state, Control.DefaultForeColor, Control.DefaultBackColor)
	{
	}

	public DrawItemEventArgs(Graphics graphics, Font font, Rectangle rect, int index, DrawItemState state, Color foreColor, Color backColor)
	{
		this.graphics = graphics;
		this.font = font;
		this.rect = rect;
		this.index = index;
		this.state = state;
		fore_color = foreColor;
		back_color = backColor;
	}

	public virtual void DrawBackground()
	{
		ThemeEngine.Current.DrawOwnerDrawBackground(this);
	}

	public virtual void DrawFocusRectangle()
	{
		ThemeEngine.Current.DrawOwnerDrawFocusRectangle(this);
	}
}
