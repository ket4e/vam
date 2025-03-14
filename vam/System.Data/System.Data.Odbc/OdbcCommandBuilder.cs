using System.ComponentModel;
using System.Data.Common;
using System.Text;

namespace System.Data.Odbc;

public sealed class OdbcCommandBuilder : DbCommandBuilder
{
	private OdbcDataAdapter _adapter;

	private DataTable _schema;

	private string _tableName;

	private OdbcCommand _insertCommand;

	private OdbcCommand _updateCommand;

	private OdbcCommand _deleteCommand;

	private bool _disposed;

	private OdbcRowUpdatingEventHandler rowUpdatingHandler;

	[DefaultValue(null)]
	[OdbcDescription("The DataAdapter for which to automatically generate OdbcCommands")]
	public new OdbcDataAdapter DataAdapter
	{
		get
		{
			return _adapter;
		}
		set
		{
			if (_adapter != value)
			{
				if (rowUpdatingHandler != null)
				{
					rowUpdatingHandler = OnRowUpdating;
				}
				if (_adapter != null)
				{
					_adapter.RowUpdating -= rowUpdatingHandler;
				}
				_adapter = value;
				if (_adapter != null)
				{
					_adapter.RowUpdating += rowUpdatingHandler;
				}
			}
		}
	}

	private OdbcCommand SelectCommand
	{
		get
		{
			if (DataAdapter == null)
			{
				return null;
			}
			return DataAdapter.SelectCommand;
		}
	}

	private DataTable Schema
	{
		get
		{
			if (_schema == null)
			{
				RefreshSchema();
			}
			return _schema;
		}
	}

	private string TableName
	{
		get
		{
			if (_tableName != string.Empty)
			{
				return _tableName;
			}
			DataRow[] array = Schema.Select("BaseTableName is not null and BaseTableName <> ''");
			if (array.Length > 1)
			{
				string text = (string)array[0]["BaseTableName"];
				DataRow[] array2 = array;
				foreach (DataRow dataRow in array2)
				{
					if ((string)dataRow["BaseTableName"] != text)
					{
						throw new InvalidOperationException("Dynamic SQL generation is not supported against multiple base tables.");
					}
				}
			}
			if (array.Length == 0)
			{
				throw new InvalidOperationException("Cannot determine the base table name. Cannot proceed");
			}
			_tableName = array[0]["BaseTableName"].ToString();
			return _tableName;
		}
	}

	private bool IsCommandGenerated => _insertCommand != null || _updateCommand != null || _deleteCommand != null;

	public OdbcCommandBuilder()
	{
	}

	public OdbcCommandBuilder(OdbcDataAdapter adapter)
		: this()
	{
		DataAdapter = adapter;
	}

	[System.MonoTODO]
	public static void DeriveParameters(OdbcCommand command)
	{
		throw new NotImplementedException();
	}

	private new void Dispose(bool disposing)
	{
		if (_disposed)
		{
			return;
		}
		if (disposing)
		{
			if (_insertCommand != null)
			{
				_insertCommand.Dispose();
			}
			if (_updateCommand != null)
			{
				_updateCommand.Dispose();
			}
			if (_deleteCommand != null)
			{
				_deleteCommand.Dispose();
			}
			if (_schema != null)
			{
				_schema.Dispose();
			}
			_insertCommand = null;
			_updateCommand = null;
			_deleteCommand = null;
			_schema = null;
		}
		_disposed = true;
	}

	private bool IsUpdatable(DataRow schemaRow)
	{
		if ((!schemaRow.IsNull("IsAutoIncrement") && (bool)schemaRow["IsAutoIncrement"]) || (!schemaRow.IsNull("IsRowVersion") && (bool)schemaRow["IsRowVersion"]) || (!schemaRow.IsNull("IsReadOnly") && (bool)schemaRow["IsReadOnly"]) || schemaRow.IsNull("BaseTableName") || ((string)schemaRow["BaseTableName"]).Length == 0)
		{
			return false;
		}
		return true;
	}

	private string GetColumnName(DataRow schemaRow)
	{
		string text = ((!schemaRow.IsNull("BaseColumnName")) ? ((string)schemaRow["BaseColumnName"]) : string.Empty);
		if (text == string.Empty)
		{
			text = ((!schemaRow.IsNull("ColumnName")) ? ((string)schemaRow["ColumnName"]) : string.Empty);
		}
		return text;
	}

	private OdbcParameter AddParameter(OdbcCommand cmd, string paramName, OdbcType odbcType, int length, string sourceColumnName, DataRowVersion rowVersion)
	{
		OdbcParameter odbcParameter = ((length < 0 || !(sourceColumnName != string.Empty)) ? cmd.Parameters.Add(paramName, odbcType) : cmd.Parameters.Add(paramName, odbcType, length, sourceColumnName));
		odbcParameter.SourceVersion = rowVersion;
		return odbcParameter;
	}

	private string CreateOptWhereClause(OdbcCommand command, int paramCount)
	{
		string[] array = new string[Schema.Rows.Count];
		int count = 0;
		foreach (DataRow row in Schema.Rows)
		{
			if (IsUpdatable(row))
			{
				string columnName = GetColumnName(row);
				if (columnName == string.Empty)
				{
					throw new InvalidOperationException("Cannot form delete command. Column name is missing!");
				}
				bool flag = row.IsNull("AllowDBNull") || (bool)row["AllowDBNull"];
				OdbcType odbcType = ((!row.IsNull("ProviderType")) ? ((OdbcType)(int)row["ProviderType"]) : OdbcType.VarChar);
				int length = ((!row.IsNull("ColumnSize")) ? ((int)row["ColumnSize"]) : (-1));
				if (flag)
				{
					array[count++] = string.Format("((? = 1 AND {0} IS NULL) OR ({0} = ?))", GetQuotedString(columnName));
					OdbcParameter odbcParameter = AddParameter(command, GetParameterName(++paramCount), OdbcType.Int, length, columnName, DataRowVersion.Original);
					odbcParameter.Value = 1;
					AddParameter(command, GetParameterName(++paramCount), odbcType, length, columnName, DataRowVersion.Original);
				}
				else
				{
					array[count++] = $"({GetQuotedString(columnName)} = ?)";
					AddParameter(command, GetParameterName(++paramCount), odbcType, length, columnName, DataRowVersion.Original);
				}
			}
		}
		return string.Join(" AND ", array, 0, count);
	}

	private void CreateNewCommand(ref OdbcCommand command)
	{
		OdbcCommand selectCommand = SelectCommand;
		if (command == null)
		{
			command = new OdbcCommand();
			command.Connection = selectCommand.Connection;
			command.CommandTimeout = selectCommand.CommandTimeout;
			command.Transaction = selectCommand.Transaction;
		}
		command.CommandType = CommandType.Text;
		command.UpdatedRowSource = UpdateRowSource.None;
		command.Parameters.Clear();
	}

	private OdbcCommand CreateInsertCommand(bool option)
	{
		CreateNewCommand(ref _insertCommand);
		string arg = $"INSERT INTO {GetQuotedString(TableName)}";
		string[] array = new string[Schema.Rows.Count];
		string[] array2 = new string[Schema.Rows.Count];
		int num = 0;
		foreach (DataRow row in Schema.Rows)
		{
			if (IsUpdatable(row))
			{
				string columnName = GetColumnName(row);
				if (columnName == string.Empty)
				{
					throw new InvalidOperationException("Cannot form insert command. Column name is missing!");
				}
				array[num] = GetQuotedString(columnName);
				array2[num++] = "?";
				OdbcType odbcType = ((!row.IsNull("ProviderType")) ? ((OdbcType)(int)row["ProviderType"]) : OdbcType.VarChar);
				int length = ((!row.IsNull("ColumnSize")) ? ((int)row["ColumnSize"]) : (-1));
				AddParameter(_insertCommand, GetParameterName(num), odbcType, length, columnName, DataRowVersion.Current);
			}
		}
		arg = string.Format("{0} ({1}) VALUES ({2})", arg, string.Join(", ", array, 0, num), string.Join(", ", array2, 0, num));
		_insertCommand.CommandText = arg;
		return _insertCommand;
	}

	public new OdbcCommand GetInsertCommand()
	{
		if (_insertCommand != null)
		{
			return _insertCommand;
		}
		if (_schema == null)
		{
			RefreshSchema();
		}
		return CreateInsertCommand(option: false);
	}

	public new OdbcCommand GetInsertCommand(bool useColumnsForParameterNames)
	{
		if (_insertCommand != null)
		{
			return _insertCommand;
		}
		if (_schema == null)
		{
			RefreshSchema();
		}
		return CreateInsertCommand(useColumnsForParameterNames);
	}

	private OdbcCommand CreateUpdateCommand(bool option)
	{
		CreateNewCommand(ref _updateCommand);
		string arg = $"UPDATE {GetQuotedString(TableName)} SET";
		string[] array = new string[Schema.Rows.Count];
		int num = 0;
		foreach (DataRow row in Schema.Rows)
		{
			if (IsUpdatable(row))
			{
				string columnName = GetColumnName(row);
				if (columnName == string.Empty)
				{
					throw new InvalidOperationException("Cannot form update command. Column name is missing!");
				}
				OdbcType odbcType = ((!row.IsNull("ProviderType")) ? ((OdbcType)(int)row["ProviderType"]) : OdbcType.VarChar);
				int length = ((!row.IsNull("ColumnSize")) ? ((int)row["ColumnSize"]) : (-1));
				array[num++] = $"{GetQuotedString(columnName)} = ?";
				AddParameter(_updateCommand, GetParameterName(num), odbcType, length, columnName, DataRowVersion.Current);
			}
		}
		string arg2 = CreateOptWhereClause(_updateCommand, num);
		arg = string.Format("{0} {1} WHERE ({2})", arg, string.Join(", ", array, 0, num), arg2);
		_updateCommand.CommandText = arg;
		return _updateCommand;
	}

	public new OdbcCommand GetUpdateCommand()
	{
		if (_updateCommand != null)
		{
			return _updateCommand;
		}
		if (_schema == null)
		{
			RefreshSchema();
		}
		return CreateUpdateCommand(option: false);
	}

	public new OdbcCommand GetUpdateCommand(bool useColumnsForParameterNames)
	{
		if (_updateCommand != null)
		{
			return _updateCommand;
		}
		if (_schema == null)
		{
			RefreshSchema();
		}
		return CreateUpdateCommand(useColumnsForParameterNames);
	}

	private OdbcCommand CreateDeleteCommand(bool option)
	{
		CreateNewCommand(ref _deleteCommand);
		string arg = $"DELETE FROM {GetQuotedString(TableName)}";
		string arg2 = CreateOptWhereClause(_deleteCommand, 0);
		arg = $"{arg} WHERE ({arg2})";
		_deleteCommand.CommandText = arg;
		return _deleteCommand;
	}

	public new OdbcCommand GetDeleteCommand()
	{
		if (_deleteCommand != null)
		{
			return _deleteCommand;
		}
		if (_schema == null)
		{
			RefreshSchema();
		}
		return CreateDeleteCommand(option: false);
	}

	public new OdbcCommand GetDeleteCommand(bool useColumnsForParameterNames)
	{
		if (_deleteCommand != null)
		{
			return _deleteCommand;
		}
		if (_schema == null)
		{
			RefreshSchema();
		}
		return CreateDeleteCommand(useColumnsForParameterNames);
	}

	private new void RefreshSchema()
	{
		if (SelectCommand == null)
		{
			throw new InvalidOperationException("SelectCommand should be valid");
		}
		if (SelectCommand.Connection == null)
		{
			throw new InvalidOperationException("SelectCommand's Connection should be valid");
		}
		CommandBehavior commandBehavior = CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo;
		if (SelectCommand.Connection.State != ConnectionState.Open)
		{
			SelectCommand.Connection.Open();
			commandBehavior |= CommandBehavior.CloseConnection;
		}
		OdbcDataReader odbcDataReader = SelectCommand.ExecuteReader(commandBehavior);
		_schema = odbcDataReader.GetSchemaTable();
		odbcDataReader.Close();
		_insertCommand = null;
		_updateCommand = null;
		_deleteCommand = null;
		_tableName = string.Empty;
	}

	protected override string GetParameterName(int parameterOrdinal)
	{
		return $"p{parameterOrdinal}";
	}

	protected override void ApplyParameterInfo(DbParameter parameter, DataRow row, StatementType statementType, bool whereClause)
	{
		OdbcParameter odbcParameter = (OdbcParameter)parameter;
		odbcParameter.Size = int.Parse(row["ColumnSize"].ToString());
		if (row["NumericPrecision"] != DBNull.Value)
		{
			odbcParameter.Precision = byte.Parse(row["NumericPrecision"].ToString());
		}
		if (row["NumericScale"] != DBNull.Value)
		{
			odbcParameter.Scale = byte.Parse(row["NumericScale"].ToString());
		}
		odbcParameter.DbType = (DbType)(int)row["ProviderType"];
	}

	protected override string GetParameterName(string parameterName)
	{
		return $"@{parameterName}";
	}

	protected override string GetParameterPlaceholder(int parameterOrdinal)
	{
		return GetParameterName(parameterOrdinal);
	}

	protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
	{
		if (!(adapter is OdbcDataAdapter))
		{
			throw new InvalidOperationException("Adapter needs to be a SqlDataAdapter");
		}
		if (rowUpdatingHandler == null)
		{
			rowUpdatingHandler = OnRowUpdating;
		}
		((OdbcDataAdapter)adapter).RowUpdating += rowUpdatingHandler;
	}

	public override string QuoteIdentifier(string unquotedIdentifier)
	{
		return QuoteIdentifier(unquotedIdentifier, null);
	}

	public string QuoteIdentifier(string unquotedIdentifier, OdbcConnection connection)
	{
		if (unquotedIdentifier == null)
		{
			throw new ArgumentNullException("unquotedIdentifier");
		}
		string text = QuotePrefix;
		string text2 = QuoteSuffix;
		if (QuotePrefix.Length == 0)
		{
			if (connection == null)
			{
				throw new InvalidOperationException("An open connection is required if QuotePrefix is not set.");
			}
			text = (text2 = GetQuoteCharacter(connection));
		}
		if (text.Length > 0 && text != " ")
		{
			string text3 = ((text2.Length <= 0) ? unquotedIdentifier : unquotedIdentifier.Replace(text2, text2 + text2));
			return text + text3 + text2;
		}
		return unquotedIdentifier;
	}

	public string UnquoteIdentifier(string quotedIdentifier, OdbcConnection connection)
	{
		return UnquoteIdentifier(quotedIdentifier);
	}

	public override string UnquoteIdentifier(string quotedIdentifier)
	{
		if (quotedIdentifier == null || quotedIdentifier.Length == 0)
		{
			return quotedIdentifier;
		}
		StringBuilder stringBuilder = new StringBuilder(quotedIdentifier.Length);
		stringBuilder.Append(quotedIdentifier);
		if (quotedIdentifier.StartsWith(QuotePrefix))
		{
			stringBuilder.Remove(0, QuotePrefix.Length);
		}
		if (quotedIdentifier.EndsWith(QuoteSuffix))
		{
			stringBuilder.Remove(stringBuilder.Length - QuoteSuffix.Length, QuoteSuffix.Length);
		}
		return stringBuilder.ToString();
	}

	private void OnRowUpdating(object sender, OdbcRowUpdatingEventArgs args)
	{
		if (args.Command != null)
		{
			return;
		}
		try
		{
			switch (args.StatementType)
			{
			case StatementType.Insert:
				args.Command = GetInsertCommand();
				break;
			case StatementType.Update:
				args.Command = GetUpdateCommand();
				break;
			case StatementType.Delete:
				args.Command = GetDeleteCommand();
				break;
			}
		}
		catch (Exception errors)
		{
			args.Errors = errors;
			args.Status = UpdateStatus.ErrorsOccurred;
		}
	}

	private string GetQuotedString(string unquotedIdentifier)
	{
		string quotePrefix = QuotePrefix;
		string quoteSuffix = QuoteSuffix;
		if (quotePrefix.Length == 0 && quoteSuffix.Length == 0)
		{
			return unquotedIdentifier;
		}
		return $"{quotePrefix}{unquotedIdentifier}{quoteSuffix}";
	}

	private string GetQuoteCharacter(OdbcConnection conn)
	{
		return conn.GetInfo(OdbcInfo.IdentifierQuoteChar);
	}
}
