namespace System.Windows.Forms;

public class DataGridViewCellStateChangedEventArgs : EventArgs
{
	private DataGridViewCell dataGridViewCell;

	private DataGridViewElementStates stateChanged;

	public DataGridViewCell Cell => dataGridViewCell;

	public DataGridViewElementStates StateChanged => stateChanged;

	public DataGridViewCellStateChangedEventArgs(DataGridViewCell dataGridViewCell, DataGridViewElementStates stateChanged)
	{
		this.dataGridViewCell = dataGridViewCell;
		this.stateChanged = stateChanged;
	}
}
