namespace System.Windows.Forms;

public class NodeLabelEditEventArgs : EventArgs
{
	private TreeNode node;

	private string label;

	private bool cancel;

	public bool CancelEdit
	{
		get
		{
			return cancel;
		}
		set
		{
			cancel = value;
			if (cancel)
			{
				node.EndEdit(cancel: true);
			}
		}
	}

	public TreeNode Node => node;

	public string Label => label;

	public NodeLabelEditEventArgs(TreeNode node)
	{
		this.node = node;
	}

	public NodeLabelEditEventArgs(TreeNode node, string label)
		: this(node)
	{
		this.label = label;
	}

	internal void SetLabel(string label)
	{
		this.label = label;
	}
}
