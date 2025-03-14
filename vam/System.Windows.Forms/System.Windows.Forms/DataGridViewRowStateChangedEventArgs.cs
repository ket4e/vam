namespace System.Windows.Forms;

public class DataGridViewRowStateChangedEventArgs : EventArgs
{
	private DataGridViewRow dataGridViewRow;

	private DataGridViewElementStates stateChanged;

	public DataGridViewRow Row => dataGridViewRow;

	public DataGridViewElementStates StateChanged => stateChanged;

	public DataGridViewRowStateChangedEventArgs(DataGridViewRow dataGridViewRow, DataGridViewElementStates stateChanged)
	{
		this.dataGridViewRow = dataGridViewRow;
		this.stateChanged = stateChanged;
	}
}
