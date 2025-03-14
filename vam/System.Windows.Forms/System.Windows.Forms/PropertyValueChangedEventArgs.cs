using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public class PropertyValueChangedEventArgs : EventArgs
{
	private GridItem changed_item;

	private object old_value;

	public GridItem ChangedItem => changed_item;

	public object OldValue => old_value;

	public PropertyValueChangedEventArgs(GridItem changedItem, object oldValue)
	{
		changed_item = changedItem;
		old_value = oldValue;
	}
}
