namespace System.Data;

public sealed class DataTableNewRowEventArgs : EventArgs
{
	private readonly DataRow _row;

	public DataRow Row => _row;

	public DataTableNewRowEventArgs(DataRow row)
	{
		_row = row;
	}
}
