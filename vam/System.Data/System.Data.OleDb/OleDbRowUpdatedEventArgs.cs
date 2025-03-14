using System.Data.Common;

namespace System.Data.OleDb;

public sealed class OleDbRowUpdatedEventArgs : RowUpdatedEventArgs
{
	public new OleDbCommand Command => (OleDbCommand)base.Command;

	public OleDbRowUpdatedEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		: base(dataRow, command, statementType, tableMapping)
	{
	}
}
