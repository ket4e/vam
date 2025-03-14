namespace System.Windows.Forms;

public class ListViewVirtualItemsSelectionRangeChangedEventArgs : EventArgs
{
	private bool is_selected;

	private int end_index;

	private int start_index;

	public int StartIndex => start_index;

	public bool IsSelected => is_selected;

	public int EndIndex => end_index;

	public ListViewVirtualItemsSelectionRangeChangedEventArgs(int startIndex, int endIndex, bool isSelected)
	{
		start_index = startIndex;
		end_index = endIndex;
		is_selected = isSelected;
	}
}
