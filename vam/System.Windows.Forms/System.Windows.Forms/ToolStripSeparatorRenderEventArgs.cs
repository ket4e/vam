using System.Drawing;

namespace System.Windows.Forms;

public class ToolStripSeparatorRenderEventArgs : ToolStripItemRenderEventArgs
{
	private bool vertical;

	public bool Vertical => vertical;

	public ToolStripSeparatorRenderEventArgs(Graphics g, ToolStripSeparator separator, bool vertical)
		: base(g, separator)
	{
		this.vertical = vertical;
	}
}
