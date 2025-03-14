namespace System.Data;

public class DataRowChangeEventArgs : EventArgs
{
	private DataRow row;

	private DataRowAction action;

	public DataRowAction Action => action;

	public DataRow Row => row;

	public DataRowChangeEventArgs(DataRow row, DataRowAction action)
	{
		this.row = row;
		this.action = action;
	}
}
