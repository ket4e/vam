namespace System.Windows.Forms;

public class DataGridViewAutoSizeColumnModeEventArgs : EventArgs
{
	private DataGridViewColumn dataGridViewColumn;

	private DataGridViewAutoSizeColumnMode previousMode;

	public DataGridViewColumn Column => dataGridViewColumn;

	public DataGridViewAutoSizeColumnMode PreviousMode => previousMode;

	public DataGridViewAutoSizeColumnModeEventArgs(DataGridViewColumn dataGridViewColumn, DataGridViewAutoSizeColumnMode previousMode)
	{
		this.dataGridViewColumn = dataGridViewColumn;
		this.previousMode = previousMode;
	}
}
