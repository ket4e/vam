namespace System.Windows.Forms;

public class ToolStripItemEventArgs : EventArgs
{
	private ToolStripItem item;

	public ToolStripItem Item => item;

	public ToolStripItemEventArgs(ToolStripItem item)
	{
		this.item = item;
	}
}
