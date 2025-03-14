using System.Drawing;

namespace System.Windows.Forms;

public class ToolStripGripRenderEventArgs : ToolStripRenderEventArgs
{
	private Rectangle grip_bounds;

	private ToolStripGripDisplayStyle grip_display_style;

	private ToolStripGripStyle grip_style;

	public Rectangle GripBounds => grip_bounds;

	public ToolStripGripDisplayStyle GripDisplayStyle => grip_display_style;

	public ToolStripGripStyle GripStyle => grip_style;

	public ToolStripGripRenderEventArgs(Graphics g, ToolStrip toolStrip)
		: base(g, toolStrip)
	{
		grip_bounds = new Rectangle(2, 0, 3, 25);
		grip_display_style = ToolStripGripDisplayStyle.Vertical;
		grip_style = ToolStripGripStyle.Visible;
	}

	internal ToolStripGripRenderEventArgs(Graphics g, ToolStrip toolStrip, Rectangle gripBounds, ToolStripGripDisplayStyle displayStyle, ToolStripGripStyle gripStyle)
		: base(g, toolStrip)
	{
		grip_bounds = gripBounds;
		grip_display_style = displayStyle;
		grip_style = gripStyle;
	}
}
