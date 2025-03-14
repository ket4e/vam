namespace System.Windows.Forms;

public class ItemChangedEventArgs : EventArgs
{
	private int index;

	public int Index => index;

	internal ItemChangedEventArgs(int index)
	{
		this.index = index;
	}
}
