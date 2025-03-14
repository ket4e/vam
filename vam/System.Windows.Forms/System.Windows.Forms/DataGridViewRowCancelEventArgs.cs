using System.ComponentModel;

namespace System.Windows.Forms;

public class DataGridViewRowCancelEventArgs : CancelEventArgs
{
	private DataGridViewRow dataGridViewRow;

	public DataGridViewRow Row => dataGridViewRow;

	public DataGridViewRowCancelEventArgs(DataGridViewRow dataGridViewRow)
	{
		this.dataGridViewRow = dataGridViewRow;
	}
}
