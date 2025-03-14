using System.ComponentModel;

namespace System.Windows.Forms;

public class MaskInputRejectedEventArgs : EventArgs
{
	private int position;

	private MaskedTextResultHint rejection_hint;

	public int Position => position;

	public MaskedTextResultHint RejectionHint => rejection_hint;

	public MaskInputRejectedEventArgs(int position, MaskedTextResultHint rejectionHint)
	{
		this.position = position;
		rejection_hint = rejectionHint;
	}
}
