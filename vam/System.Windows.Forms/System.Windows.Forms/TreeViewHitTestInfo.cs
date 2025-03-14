namespace System.Windows.Forms;

public class TreeViewHitTestInfo
{
	private TreeNode node;

	private TreeViewHitTestLocations location;

	public TreeNode Node => node;

	public TreeViewHitTestLocations Location => location;

	public TreeViewHitTestInfo(TreeNode hitNode, TreeViewHitTestLocations hitLocation)
	{
		node = hitNode;
		location = hitLocation;
	}
}
