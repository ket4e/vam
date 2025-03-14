namespace System.Windows.Forms;

public class ToolStripItemClickedEventArgs : EventArgs
{
	private ToolStripItem clicked_item;

	public ToolStripItem ClickedItem => clicked_item;

	public ToolStripItemClickedEventArgs(ToolStripItem clickedItem)
	{
		clicked_item = clickedItem;
	}
}
