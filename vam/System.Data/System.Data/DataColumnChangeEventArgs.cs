namespace System.Data;

public class DataColumnChangeEventArgs : EventArgs
{
	private DataColumn _column;

	private DataRow _row;

	private object _proposedValue;

	public DataColumn Column => _column;

	public object ProposedValue
	{
		get
		{
			return _proposedValue;
		}
		set
		{
			_proposedValue = value;
		}
	}

	public DataRow Row => _row;

	public DataColumnChangeEventArgs(DataRow row, DataColumn column, object value)
	{
		Initialize(row, column, value);
	}

	internal DataColumnChangeEventArgs()
	{
	}

	internal void Initialize(DataRow row, DataColumn column, object value)
	{
		_column = column;
		_row = row;
		_proposedValue = value;
	}
}
