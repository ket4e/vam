namespace System.Data;

public sealed class DataTableClearEventArgs : EventArgs
{
	private readonly DataTable _table;

	public DataTable Table => _table;

	public string TableName => _table.TableName;

	public string TableNamespace => _table.Namespace;

	public DataTableClearEventArgs(DataTable table)
	{
		_table = table;
	}
}
