namespace System.Windows.Forms;

public class FormClosedEventArgs : EventArgs
{
	private CloseReason close_reason;

	public CloseReason CloseReason => close_reason;

	public FormClosedEventArgs(CloseReason closeReason)
	{
		close_reason = closeReason;
	}
}
