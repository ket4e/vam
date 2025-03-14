using System.ComponentModel;
using System.Data.Common;

namespace System.Data.Odbc;

[DefaultEvent("RecordsAffected")]
[Designer("Microsoft.VSDesigner.Data.VS.OdbcCommandDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[ToolboxItem("System.Drawing.Design.ToolboxItem, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
public sealed class OdbcCommand : DbCommand, ICloneable
{
	private const int DEFAULT_COMMAND_TIMEOUT = 30;

	private string commandText;

	private int timeout;

	private CommandType commandType;

	private UpdateRowSource updateRowSource;

	private OdbcConnection connection;

	private OdbcTransaction transaction;

	private OdbcParameterCollection _parameters;

	private bool designTimeVisible;

	private bool prepared;

	private IntPtr hstmt = IntPtr.Zero;

	private bool disposed;

	internal IntPtr hStmt => hstmt;

	[DefaultValue("")]
	[OdbcDescription("Command text to execute")]
	[RefreshProperties(RefreshProperties.All)]
	[Editor("Microsoft.VSDesigner.Data.Odbc.Design.OdbcCommandTextEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[OdbcCategory("Data")]
	public override string CommandText
	{
		get
		{
			if (commandText == null)
			{
				return string.Empty;
			}
			return commandText;
		}
		set
		{
			prepared = false;
			commandText = value;
		}
	}

	[OdbcDescription("Time to wait for command to execute")]
	public override int CommandTimeout
	{
		get
		{
			return timeout;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentException("The property value assigned is less than 0.", "CommandTimeout");
			}
			timeout = value;
		}
	}

	[OdbcCategory("Data")]
	[RefreshProperties(RefreshProperties.All)]
	[OdbcDescription("How to interpret the CommandText")]
	[DefaultValue("Text")]
	public override CommandType CommandType
	{
		get
		{
			return commandType;
		}
		set
		{
			ExceptionHelper.CheckEnumValue(typeof(CommandType), value);
			commandType = value;
		}
	}

	[Editor("Microsoft.VSDesigner.Data.Design.DbConnectionEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DefaultValue(null)]
	public new OdbcConnection Connection
	{
		get
		{
			return DbConnection as OdbcConnection;
		}
		set
		{
			DbConnection = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DefaultValue(true)]
	[DesignOnly(true)]
	public override bool DesignTimeVisible
	{
		get
		{
			return designTimeVisible;
		}
		set
		{
			designTimeVisible = value;
		}
	}

	[OdbcCategory("Data")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[OdbcDescription("The parameters collection")]
	public new OdbcParameterCollection Parameters => base.Parameters as OdbcParameterCollection;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[OdbcDescription("The transaction used by the command")]
	[Browsable(false)]
	public new OdbcTransaction Transaction
	{
		get
		{
			return transaction;
		}
		set
		{
			transaction = value;
		}
	}

	[DefaultValue(UpdateRowSource.Both)]
	[OdbcDescription("When used by a DataAdapter.Update, how command results are applied to the current DataRow")]
	[OdbcCategory("Behavior")]
	public override UpdateRowSource UpdatedRowSource
	{
		get
		{
			return updateRowSource;
		}
		set
		{
			ExceptionHelper.CheckEnumValue(typeof(UpdateRowSource), value);
			updateRowSource = value;
		}
	}

	protected override DbConnection DbConnection
	{
		get
		{
			return connection;
		}
		set
		{
			connection = (OdbcConnection)value;
		}
	}

	protected override DbParameterCollection DbParameterCollection => _parameters;

	protected override DbTransaction DbTransaction
	{
		get
		{
			return transaction;
		}
		set
		{
			transaction = (OdbcTransaction)value;
		}
	}

	public OdbcCommand()
	{
		timeout = 30;
		commandType = CommandType.Text;
		_parameters = new OdbcParameterCollection();
		designTimeVisible = true;
		updateRowSource = UpdateRowSource.Both;
	}

	public OdbcCommand(string cmdText)
		: this()
	{
		commandText = cmdText;
	}

	public OdbcCommand(string cmdText, OdbcConnection connection)
		: this(cmdText)
	{
		Connection = connection;
	}

	public OdbcCommand(string cmdText, OdbcConnection connection, OdbcTransaction transaction)
		: this(cmdText, connection)
	{
		Transaction = transaction;
	}

	object ICloneable.Clone()
	{
		OdbcCommand odbcCommand = new OdbcCommand();
		odbcCommand.CommandText = CommandText;
		odbcCommand.CommandTimeout = CommandTimeout;
		odbcCommand.CommandType = CommandType;
		odbcCommand.Connection = Connection;
		odbcCommand.DesignTimeVisible = DesignTimeVisible;
		foreach (OdbcParameter parameter in Parameters)
		{
			odbcCommand.Parameters.Add(parameter);
		}
		odbcCommand.Transaction = Transaction;
		return odbcCommand;
	}

	public override void Cancel()
	{
		if (hstmt != IntPtr.Zero)
		{
			OdbcReturn odbcReturn = libodbc.SQLCancel(hstmt);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw connection.CreateOdbcException(OdbcHandleType.Stmt, hstmt);
			}
			return;
		}
		throw new InvalidOperationException();
	}

	protected override DbParameter CreateDbParameter()
	{
		return CreateParameter();
	}

	public new OdbcParameter CreateParameter()
	{
		return new OdbcParameter();
	}

	internal void Unlink()
	{
		if (!disposed)
		{
			FreeStatement(unlink: false);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposed)
		{
			FreeStatement();
			CommandText = null;
			Connection = null;
			Transaction = null;
			Parameters.Clear();
			disposed = true;
		}
	}

	private IntPtr ReAllocStatment()
	{
		if (hstmt != IntPtr.Zero)
		{
			FreeStatement();
		}
		else
		{
			Connection.Link(this);
		}
		OdbcReturn odbcReturn = libodbc.SQLAllocHandle(OdbcHandleType.Stmt, Connection.hDbc, ref hstmt);
		if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
		{
			throw connection.CreateOdbcException(OdbcHandleType.Dbc, Connection.hDbc);
		}
		disposed = false;
		return hstmt;
	}

	private void FreeStatement()
	{
		FreeStatement(unlink: true);
	}

	private void FreeStatement(bool unlink)
	{
		prepared = false;
		if (!(hstmt == IntPtr.Zero))
		{
			if (unlink)
			{
				Connection.Unlink(this);
			}
			OdbcReturn odbcReturn = libodbc.SQLFreeStmt(hstmt, libodbc.SQLFreeStmtOptions.Close);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw connection.CreateOdbcException(OdbcHandleType.Stmt, hstmt);
			}
			odbcReturn = libodbc.SQLFreeHandle(3, hstmt);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw connection.CreateOdbcException(OdbcHandleType.Stmt, hstmt);
			}
			hstmt = IntPtr.Zero;
		}
	}

	private void ExecSQL(CommandBehavior behavior, bool createReader, string sql)
	{
		OdbcReturn odbcReturn;
		if (!prepared && Parameters.Count == 0)
		{
			ReAllocStatment();
			odbcReturn = libodbc.SQLExecDirect(hstmt, sql, -3);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo && odbcReturn != OdbcReturn.NoData)
			{
				throw connection.CreateOdbcException(OdbcHandleType.Stmt, hstmt);
			}
			return;
		}
		if (!prepared)
		{
			Prepare();
		}
		BindParameters();
		odbcReturn = libodbc.SQLExecute(hstmt);
		if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
		{
			throw connection.CreateOdbcException(OdbcHandleType.Stmt, hstmt);
		}
	}

	internal void FreeIfNotPrepared()
	{
		if (!prepared)
		{
			FreeStatement();
		}
	}

	public override int ExecuteNonQuery()
	{
		return ExecuteNonQuery("ExecuteNonQuery", CommandBehavior.Default, createReader: false);
	}

	private int ExecuteNonQuery(string method, CommandBehavior behavior, bool createReader)
	{
		int num = 0;
		if (Connection == null)
		{
			throw new InvalidOperationException($"{method}: Connection is not set.");
		}
		if (Connection.State == ConnectionState.Closed)
		{
			throw new InvalidOperationException($"{method}: Connection state is closed");
		}
		if (CommandText.Length == 0)
		{
			throw new InvalidOperationException($"{method}: CommandText is not set.");
		}
		ExecSQL(behavior, createReader, CommandText);
		if (CommandText.ToUpper().IndexOf("UPDATE") != -1 || CommandText.ToUpper().IndexOf("INSERT") != -1 || CommandText.ToUpper().IndexOf("DELETE") != -1)
		{
			int RowCount = 0;
			OdbcReturn odbcReturn = libodbc.SQLRowCount(hstmt, ref RowCount);
			num = RowCount;
		}
		else
		{
			num = -1;
		}
		if (!createReader && !prepared)
		{
			FreeStatement();
		}
		return num;
	}

	public override void Prepare()
	{
		ReAllocStatment();
		OdbcReturn odbcReturn = libodbc.SQLPrepare(hstmt, CommandText, CommandText.Length);
		if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
		{
			throw connection.CreateOdbcException(OdbcHandleType.Stmt, hstmt);
		}
		prepared = true;
	}

	private void BindParameters()
	{
		int num = 1;
		foreach (OdbcParameter parameter in Parameters)
		{
			parameter.Bind(this, hstmt, num);
			parameter.CopyValue();
			num++;
		}
	}

	public new OdbcDataReader ExecuteReader()
	{
		return ExecuteReader(CommandBehavior.Default);
	}

	protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
	{
		return ExecuteReader(behavior);
	}

	public new OdbcDataReader ExecuteReader(CommandBehavior behavior)
	{
		return ExecuteReader("ExecuteReader", behavior);
	}

	private OdbcDataReader ExecuteReader(string method, CommandBehavior behavior)
	{
		int recordAffected = ExecuteNonQuery(method, behavior, createReader: true);
		return new OdbcDataReader(this, behavior, recordAffected);
	}

	public override object ExecuteScalar()
	{
		object result = null;
		OdbcDataReader odbcDataReader = ExecuteReader("ExecuteScalar", CommandBehavior.Default);
		try
		{
			if (odbcDataReader.Read())
			{
				result = odbcDataReader[0];
			}
		}
		finally
		{
			odbcDataReader.Close();
		}
		return result;
	}

	public void ResetCommandTimeout()
	{
		CommandTimeout = 30;
	}
}
