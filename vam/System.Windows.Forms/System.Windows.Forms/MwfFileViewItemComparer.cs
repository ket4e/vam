using System.Collections;

namespace System.Windows.Forms;

internal class MwfFileViewItemComparer : IComparer
{
	private int column_index;

	private bool asc;

	public int ColumnIndex
	{
		get
		{
			return column_index;
		}
		set
		{
			column_index = value;
		}
	}

	public bool Ascendent
	{
		get
		{
			return asc;
		}
		set
		{
			asc = value;
		}
	}

	public MwfFileViewItemComparer(bool asc)
	{
		this.asc = asc;
	}

	public int Compare(object a, object b)
	{
		ListViewItem listViewItem = (ListViewItem)a;
		ListViewItem listViewItem2 = (ListViewItem)b;
		if (asc)
		{
			return string.Compare(listViewItem.SubItems[column_index].Text, listViewItem2.SubItems[column_index].Text);
		}
		return string.Compare(listViewItem2.SubItems[column_index].Text, listViewItem.SubItems[column_index].Text);
	}
}
