using System.ComponentModel;

namespace System.Windows.Forms;

public class DataGridViewBindingCompleteEventArgs : EventArgs
{
	private ListChangedType listChangedType;

	public ListChangedType ListChangedType => listChangedType;

	public DataGridViewBindingCompleteEventArgs(ListChangedType listChangedType)
	{
		this.listChangedType = listChangedType;
	}
}
