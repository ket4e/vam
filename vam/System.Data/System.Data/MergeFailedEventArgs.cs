namespace System.Data;

public class MergeFailedEventArgs : EventArgs
{
	private readonly DataTable data_table;

	private readonly string conflict;

	public DataTable Table => data_table;

	public string Conflict => conflict;

	public MergeFailedEventArgs(DataTable table, string conflict)
	{
		data_table = table;
		this.conflict = conflict;
	}
}
