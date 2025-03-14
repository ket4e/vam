using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public class TreeNodeMouseHoverEventArgs : EventArgs
{
	private TreeNode node;

	public TreeNode Node => node;

	public TreeNodeMouseHoverEventArgs(TreeNode node)
	{
		this.node = node;
	}
}
