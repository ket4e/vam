using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public class ListViewItemMouseHoverEventArgs : EventArgs
{
	private ListViewItem item;

	public ListViewItem Item => item;

	public ListViewItemMouseHoverEventArgs(ListViewItem item)
	{
		this.item = item;
	}
}
