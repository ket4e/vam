using System.ComponentModel;

namespace System.Windows.Forms;

public class FormClosingEventArgs : CancelEventArgs
{
	private CloseReason close_reason;

	public CloseReason CloseReason => close_reason;

	public FormClosingEventArgs(CloseReason closeReason, bool cancel)
		: base(cancel)
	{
		close_reason = closeReason;
	}
}
