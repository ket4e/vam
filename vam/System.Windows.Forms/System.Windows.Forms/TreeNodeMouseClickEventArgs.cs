namespace System.Windows.Forms;

public class TreeNodeMouseClickEventArgs : MouseEventArgs
{
	private TreeNode node;

	public TreeNode Node => node;

	public TreeNodeMouseClickEventArgs(TreeNode node, MouseButtons button, int clicks, int x, int y)
		: base(button, clicks, x, y, 0)
	{
		this.node = node;
	}
}
