namespace System.Windows.Forms;

public class ListViewItemSelectionChangedEventArgs : EventArgs
{
	private bool is_selected;

	private ListViewItem item;

	private int item_index;

	public ListViewItem Item => item;

	public bool IsSelected => is_selected;

	public int ItemIndex => item_index;

	public ListViewItemSelectionChangedEventArgs(ListViewItem item, int itemIndex, bool isSelected)
	{
		this.item = item;
		item_index = itemIndex;
		is_selected = isSelected;
	}
}
