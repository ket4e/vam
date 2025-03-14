namespace System.Windows.Forms;

public class RetrieveVirtualItemEventArgs : EventArgs
{
	private ListViewItem item;

	private int item_index;

	public ListViewItem Item
	{
		get
		{
			return item;
		}
		set
		{
			item = value;
		}
	}

	public int ItemIndex => item_index;

	public RetrieveVirtualItemEventArgs(int itemIndex)
	{
		item_index = itemIndex;
	}
}
