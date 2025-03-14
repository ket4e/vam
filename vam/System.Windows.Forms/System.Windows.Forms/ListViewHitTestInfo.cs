namespace System.Windows.Forms;

public class ListViewHitTestInfo
{
	private ListViewItem item;

	private ListViewItem.ListViewSubItem subItem;

	private ListViewHitTestLocations location = ListViewHitTestLocations.None;

	public ListViewItem Item => item;

	public ListViewHitTestLocations Location => location;

	public ListViewItem.ListViewSubItem SubItem => subItem;

	public ListViewHitTestInfo(ListViewItem hitItem, ListViewItem.ListViewSubItem hitSubItem, ListViewHitTestLocations hitLocation)
	{
		item = hitItem;
		subItem = hitSubItem;
		location = hitLocation;
	}
}
