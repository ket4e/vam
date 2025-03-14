namespace System.Windows.Forms;

public class TreeViewEventArgs : EventArgs
{
	private TreeNode node;

	private TreeViewAction action;

	public TreeViewAction Action => action;

	public TreeNode Node => node;

	public TreeViewEventArgs(TreeNode node)
	{
		this.node = node;
	}

	public TreeViewEventArgs(TreeNode node, TreeViewAction action)
		: this(node)
	{
		this.action = action;
	}
}
