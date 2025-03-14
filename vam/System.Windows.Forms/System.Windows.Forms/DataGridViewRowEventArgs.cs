namespace System.Windows.Forms;

public class DataGridViewRowEventArgs : EventArgs
{
	private DataGridViewRow dataGridViewRow;

	public DataGridViewRow Row => dataGridViewRow;

	public DataGridViewRowEventArgs(DataGridViewRow dataGridViewRow)
	{
		this.dataGridViewRow = dataGridViewRow;
	}
}
