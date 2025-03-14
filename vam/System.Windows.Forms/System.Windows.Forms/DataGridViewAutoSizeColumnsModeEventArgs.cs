namespace System.Windows.Forms;

public class DataGridViewAutoSizeColumnsModeEventArgs : EventArgs
{
	private DataGridViewAutoSizeColumnMode[] previousModes;

	public DataGridViewAutoSizeColumnMode[] PreviousModes => previousModes;

	public DataGridViewAutoSizeColumnsModeEventArgs(DataGridViewAutoSizeColumnMode[] previousModes)
	{
		this.previousModes = previousModes;
	}
}
