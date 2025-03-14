using System.Drawing;

namespace System.Windows.Forms;

public class DrawTreeNodeEventArgs : EventArgs
{
	private Rectangle bounds;

	private bool draw_default;

	private Graphics graphics;

	private TreeNode node;

	private TreeNodeStates state;

	public Rectangle Bounds => bounds;

	public bool DrawDefault
	{
		get
		{
			return draw_default;
		}
		set
		{
			draw_default = value;
		}
	}

	public Graphics Graphics => graphics;

	public TreeNode Node => node;

	public TreeNodeStates State => state;

	public DrawTreeNodeEventArgs(Graphics graphics, TreeNode node, Rectangle bounds, TreeNodeStates state)
	{
		this.bounds = bounds;
		draw_default = false;
		this.graphics = graphics;
		this.node = node;
		this.state = state;
	}
}
