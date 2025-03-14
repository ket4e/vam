using System.Runtime.InteropServices;
using System.Windows.Forms.Design;

namespace System.Windows.Forms;

[ComVisible(true)]
public class PropertyTabChangedEventArgs : EventArgs
{
	private PropertyTab old_tab;

	private PropertyTab new_tab;

	public PropertyTab NewTab => new_tab;

	public PropertyTab OldTab => old_tab;

	public PropertyTabChangedEventArgs(PropertyTab oldTab, PropertyTab newTab)
	{
		old_tab = oldTab;
		new_tab = newTab;
	}
}
