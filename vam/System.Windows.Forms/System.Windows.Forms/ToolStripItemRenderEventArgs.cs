using System.Drawing;

namespace System.Windows.Forms;

public class ToolStripItemRenderEventArgs : EventArgs
{
	private Graphics graphics;

	private ToolStripItem item;

	public Graphics Graphics => graphics;

	public ToolStripItem Item => item;

	public ToolStrip ToolStrip => item.Owner;

	public ToolStripItemRenderEventArgs(Graphics g, ToolStripItem item)
	{
		graphics = g;
		this.item = item;
	}
}
