namespace System.Windows.Forms;

public class DataGridViewColumnStateChangedEventArgs : EventArgs
{
	private DataGridViewColumn dataGridViewColumn;

	private DataGridViewElementStates stateChanged;

	public DataGridViewColumn Column => dataGridViewColumn;

	public DataGridViewElementStates StateChanged => stateChanged;

	public DataGridViewColumnStateChangedEventArgs(DataGridViewColumn dataGridViewColumn, DataGridViewElementStates stateChanged)
	{
		this.dataGridViewColumn = dataGridViewColumn;
		this.stateChanged = stateChanged;
	}
}
