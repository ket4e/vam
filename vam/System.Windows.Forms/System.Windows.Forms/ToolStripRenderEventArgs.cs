using System.Drawing;

namespace System.Windows.Forms;

public class ToolStripRenderEventArgs : EventArgs
{
	private Rectangle affected_bounds;

	private Color back_color;

	private Rectangle connected_area;

	private Graphics graphics;

	private ToolStrip tool_strip;

	public Rectangle AffectedBounds => affected_bounds;

	public Color BackColor => back_color;

	public Rectangle ConnectedArea => connected_area;

	public Graphics Graphics => graphics;

	public ToolStrip ToolStrip => tool_strip;

	internal Rectangle InternalConnectedArea
	{
		set
		{
			connected_area = value;
		}
	}

	public ToolStripRenderEventArgs(Graphics g, ToolStrip toolStrip)
		: this(g, toolStrip, new Rectangle(0, 0, 100, 25), SystemColors.Control)
	{
	}

	public ToolStripRenderEventArgs(Graphics g, ToolStrip toolStrip, Rectangle affectedBounds, Color backColor)
	{
		graphics = g;
		tool_strip = toolStrip;
		affected_bounds = affectedBounds;
		back_color = backColor;
	}
}
