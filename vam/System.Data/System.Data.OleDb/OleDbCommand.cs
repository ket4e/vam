using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.InteropServices;

namespace System.Data.OleDb;

[Designer("Microsoft.VSDesigner.Data.VS.OleDbCommandDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[DefaultEvent("RecordsAffected")]
[ToolboxItem("System.Drawing.Design.ToolboxItem, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
public sealed class OleDbCommand : DbCommand, IDisposable, IDbCommand, ICloneable
{
	private const int DEFAULT_COMMAND_TIMEOUT = 30;

	private string commandText;

	private int timeout;

	private CommandType commandType;

	private OleDbConnection connection;

	private OleDbParameterCollection parameters;

	private OleDbTransaction transaction;

	private bool designTimeVisible;

	private OleDbDataReader dataReader;

	private CommandBehavior behavior;

	private IntPtr gdaCommand;

	private UpdateRowSource updatedRowSource;

	private bool disposed;

	IDbConnection IDbCommand.Connection
	{
		get
		{
			return Connection;
		}
		set
		{
			Connection = (OleDbConnection)value;
		}
	}

	IDataParameterCollection IDbCommand.Parameters => Parameters;

	IDbTransaction IDbCommand.Transaction
	{
		get
		{
			return Transaction;
		}
		set
		{
			Transaction = (OleDbTransaction)value;
		}
	}

	[DataCategory("Data")]
	[RefreshProperties(RefreshProperties.All)]
	[DefaultValue("")]
	[Editor("Microsoft.VSDesigner.Data.ADO.Design.OleDbCommandTextEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
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
			commandText = value;
		}
	}

	public override int CommandTimeout
	{
		get
		{
			return timeout;
		}
		set
		{
			timeout = value;
		}
	}

	[DefaultValue("Text")]
	[DataCategory("Data")]
	[RefreshProperties(RefreshProperties.All)]
	public override CommandType CommandType
	{
		get
		{
			return commandType;
		}
		set
		{
			commandType = value;
		}
	}

	[Editor("Microsoft.VSDesigner.Data.Design.DbConnectionEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DataCategory("Behavior")]
	[DefaultValue(null)]
	public new OleDbConnection Connection
	{
		get
		{
			return connection;
		}
		set
		{
			connection = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignOnly(true)]
	[DefaultValue(true)]
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

	[DataCategory("Data")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new OleDbParameterCollection Parameters
	{
		get
		{
			return parameters;
		}
		internal set
		{
			parameters = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public new OleDbTransaction Transaction
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

	[DataCategory("Behavior")]
	[System.MonoTODO]
	[DefaultValue(UpdateRowSource.Both)]
	public override UpdateRowSource UpdatedRowSource
	{
		get
		{
			return updatedRowSource;
		}
		set
		{
			ExceptionHelper.CheckEnumValue(typeof(UpdateRowSource), value);
			updatedRowSource = value;
		}
	}

	protected override DbConnection DbConnection
	{
		get
		{
			return Connection;
		}
		set
		{
			Connection = (OleDbConnection)value;
		}
	}

	protected override DbParameterCollection DbParameterCollection => Parameters;

	protected override DbTransaction DbTransaction
	{
		get
		{
			return Transaction;
		}
		set
		{
			Transaction = (OleDbTransaction)value;
		}
	}

	public OleDbCommand()
	{
		timeout = 30;
		commandType = CommandType.Text;
		parameters = new OleDbParameterCollection();
		behavior = CommandBehavior.Default;
		gdaCommand = IntPtr.Zero;
		designTimeVisible = true;
		updatedRowSource = UpdateRowSource.Both;
	}

	public OleDbCommand(string cmdText)
		: this()
	{
		CommandText = cmdText;
	}

	public OleDbCommand(string cmdText, OleDbConnection connection)
		: this(cmdText)
	{
		Connection = connection;
	}

	public OleDbCommand(string cmdText, OleDbConnection connection, OleDbTransaction transaction)
		: this(cmdText, connection)
	{
		this.transaction = transaction;
	}

	IDataReader IDbCommand.ExecuteReader()
	{
		return ExecuteReader();
	}

	IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
	{
		return ExecuteReader(behavior);
	}

	object ICloneable.Clone()
	{
		return Clone();
	}

	[System.MonoTODO]
	public override void Cancel()
	{
		throw new NotImplementedException();
	}

	public new OleDbParameter CreateParameter()
	{
		return new OleDbParameter();
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposed)
		{
			Connection = null;
			Transaction = null;
			disposed = true;
		}
	}

	private void SetupGdaCommand()
	{
		GdaCommandType type = commandType switch
		{
			CommandType.TableDirect => GdaCommandType.Table, 
			CommandType.StoredProcedure => GdaCommandType.Procedure, 
			_ => GdaCommandType.Sql, 
		};
		if (gdaCommand != IntPtr.Zero)
		{
			libgda.gda_command_set_text(gdaCommand, CommandText);
			libgda.gda_command_set_command_type(gdaCommand, type);
		}
		else
		{
			gdaCommand = libgda.gda_command_new(CommandText, type, (GdaCommandOptions)0);
		}
	}

	public override int ExecuteNonQuery()
	{
		if (connection == null)
		{
			throw new InvalidOperationException("connection == null");
		}
		if (connection.State == ConnectionState.Closed)
		{
			throw new InvalidOperationException("State == Closed");
		}
		IntPtr gdaConnection = connection.GdaConnection;
		IntPtr gdaParameterList = parameters.GdaParameterList;
		SetupGdaCommand();
		return libgda.gda_connection_execute_non_query(gdaConnection, gdaCommand, gdaParameterList);
	}

	public new OleDbDataReader ExecuteReader()
	{
		return ExecuteReader(behavior);
	}

	public new OleDbDataReader ExecuteReader(CommandBehavior behavior)
	{
		ArrayList arrayList = new ArrayList();
		if (connection.State != ConnectionState.Open)
		{
			throw new InvalidOperationException("State != Open");
		}
		this.behavior = behavior;
		IntPtr gdaConnection = connection.GdaConnection;
		IntPtr gdaParameterList = parameters.GdaParameterList;
		SetupGdaCommand();
		IntPtr intPtr = libgda.gda_connection_execute_command(gdaConnection, gdaCommand, gdaParameterList);
		if (intPtr != IntPtr.Zero)
		{
			for (GdaList gdaList = (GdaList)Marshal.PtrToStructure(intPtr, typeof(GdaList)); gdaList != null; gdaList = (GdaList)Marshal.PtrToStructure(gdaList.next, typeof(GdaList)))
			{
				arrayList.Add(gdaList.data);
				if (gdaList.next == IntPtr.Zero)
				{
					break;
				}
			}
			dataReader = new OleDbDataReader(this, arrayList);
			dataReader.NextResult();
		}
		return dataReader;
	}

	public override object ExecuteScalar()
	{
		SetupGdaCommand();
		OleDbDataReader oleDbDataReader = ExecuteReader();
		if (oleDbDataReader == null)
		{
			return null;
		}
		if (!oleDbDataReader.Read())
		{
			oleDbDataReader.Close();
			return null;
		}
		object value = oleDbDataReader.GetValue(0);
		oleDbDataReader.Close();
		return value;
	}

	public OleDbCommand Clone()
	{
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.CommandText = CommandText;
		oleDbCommand.CommandTimeout = CommandTimeout;
		oleDbCommand.CommandType = CommandType;
		oleDbCommand.Connection = Connection;
		oleDbCommand.DesignTimeVisible = DesignTimeVisible;
		oleDbCommand.Parameters = Parameters;
		oleDbCommand.Transaction = Transaction;
		return oleDbCommand;
	}

	[System.MonoTODO]
	public override void Prepare()
	{
		throw new NotImplementedException();
	}

	public void ResetCommandTimeout()
	{
		timeout = 30;
	}

	protected override DbParameter CreateDbParameter()
	{
		return CreateParameter();
	}

	protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
	{
		return ExecuteReader(behavior);
	}
}
