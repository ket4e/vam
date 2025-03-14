using System.ComponentModel;

namespace System.Windows.Forms;

public class ToolStripDropDownClosingEventArgs : CancelEventArgs
{
	private ToolStripDropDownCloseReason close_reason;

	public ToolStripDropDownCloseReason CloseReason => close_reason;

	public ToolStripDropDownClosingEventArgs(ToolStripDropDownCloseReason reason)
	{
		close_reason = reason;
	}
}
