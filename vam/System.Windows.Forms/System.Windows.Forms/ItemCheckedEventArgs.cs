namespace System.Windows.Forms;

public class ItemCheckedEventArgs : EventArgs
{
	private ListViewItem item;

	public ListViewItem Item => item;

	public ItemCheckedEventArgs(ListViewItem item)
	{
		this.item = item;
	}
}
