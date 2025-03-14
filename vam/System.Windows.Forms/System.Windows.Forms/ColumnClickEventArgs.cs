namespace System.Windows.Forms;

public class ColumnClickEventArgs : EventArgs
{
	private int column;

	public int Column => column;

	public ColumnClickEventArgs(int column)
	{
		this.column = column;
	}
}
