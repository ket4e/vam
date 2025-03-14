namespace System.Windows.Forms;

public class DataGridViewAutoSizeModeEventArgs : EventArgs
{
	private bool previousModeAutoSized;

	public bool PreviousModeAutoSized => previousModeAutoSized;

	public DataGridViewAutoSizeModeEventArgs(bool previousModeAutoSized)
	{
		this.previousModeAutoSized = previousModeAutoSized;
	}
}
