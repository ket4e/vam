namespace System.Windows.Forms;

public class ToolStripDropDownClosedEventArgs : EventArgs
{
	private ToolStripDropDownCloseReason close_reason;

	public ToolStripDropDownCloseReason CloseReason => close_reason;

	public ToolStripDropDownClosedEventArgs(ToolStripDropDownCloseReason reason)
	{
		close_reason = reason;
	}
}
