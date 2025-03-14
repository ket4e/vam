using System.Drawing;

namespace System.Windows.Forms;

public class ToolStripArrowRenderEventArgs : EventArgs
{
	private Color arrow_color;

	private Rectangle arrow_rectangle;

	private ArrowDirection arrow_direction;

	private Graphics graphics;

	private ToolStripItem tool_strip_item;

	public Color ArrowColor
	{
		get
		{
			return arrow_color;
		}
		set
		{
			arrow_color = value;
		}
	}

	public Rectangle ArrowRectangle
	{
		get
		{
			return arrow_rectangle;
		}
		set
		{
			arrow_rectangle = value;
		}
	}

	public ArrowDirection Direction
	{
		get
		{
			return arrow_direction;
		}
		set
		{
			arrow_direction = value;
		}
	}

	public Graphics Graphics => graphics;

	public ToolStripItem Item => tool_strip_item;

	public ToolStripArrowRenderEventArgs(Graphics g, ToolStripItem toolStripItem, Rectangle arrowRectangle, Color arrowColor, ArrowDirection arrowDirection)
	{
		graphics = g;
		tool_strip_item = toolStripItem;
		arrow_rectangle = arrowRectangle;
		arrow_color = arrowColor;
		arrow_direction = arrowDirection;
	}
}
