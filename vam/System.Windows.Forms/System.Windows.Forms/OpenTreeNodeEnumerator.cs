using System.Collections;

namespace System.Windows.Forms;

internal class OpenTreeNodeEnumerator : IEnumerator
{
	private TreeNode start;

	private TreeNode current;

	private bool started;

	public object Current => current;

	public TreeNode CurrentNode => current;

	public OpenTreeNodeEnumerator(TreeNode start)
	{
		this.start = start;
	}

	public bool MoveNext()
	{
		if (!started)
		{
			started = true;
			current = start;
			return current != null;
		}
		if (current.is_expanded && current.Nodes.Count > 0)
		{
			current = current.Nodes[0];
			return true;
		}
		TreeNode parent = current;
		TreeNode nextNode = current.NextNode;
		while (nextNode == null)
		{
			if (parent.parent == null)
			{
				return false;
			}
			parent = parent.parent;
			if (parent.parent != null)
			{
				nextNode = parent.NextNode;
			}
		}
		current = nextNode;
		return true;
	}

	public bool MovePrevious()
	{
		if (!started)
		{
			started = true;
			current = start;
			return current != null;
		}
		if (current.PrevNode != null)
		{
			TreeNode treeNode = current.PrevNode;
			for (TreeNode treeNode2 = treeNode; treeNode2 != null; treeNode2 = treeNode2.LastNode)
			{
				treeNode = treeNode2;
				if (!treeNode2.is_expanded)
				{
					break;
				}
			}
			current = treeNode;
			return true;
		}
		if (current.Parent == null)
		{
			return false;
		}
		current = current.Parent;
		return true;
	}

	public void Reset()
	{
		started = false;
	}
}
