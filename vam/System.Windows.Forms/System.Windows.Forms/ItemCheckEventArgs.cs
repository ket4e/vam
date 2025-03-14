using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public class ItemCheckEventArgs : EventArgs
{
	private CheckState currentValue;

	private int index;

	private CheckState newValue;

	public CheckState CurrentValue => currentValue;

	public int Index => index;

	public CheckState NewValue
	{
		get
		{
			return newValue;
		}
		set
		{
			newValue = value;
		}
	}

	public ItemCheckEventArgs(int index, CheckState newCheckValue, CheckState currentValue)
	{
		this.index = index;
		newValue = newCheckValue;
		this.currentValue = currentValue;
	}
}
