namespace System.Windows.Forms;

public class DataGridViewColumnEventArgs : EventArgs
{
	private DataGridViewColumn dataGridViewColumn;

	public DataGridViewColumn Column => dataGridViewColumn;

	public DataGridViewColumnEventArgs(DataGridViewColumn dataGridViewColumn)
	{
		this.dataGridViewColumn = dataGridViewColumn;
	}
}
