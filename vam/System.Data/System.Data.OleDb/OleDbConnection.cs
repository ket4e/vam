using System.ComponentModel;
using System.Data.Common;
using System.EnterpriseServices;
using System.Transactions;

namespace System.Data.OleDb;

[DefaultEvent("InfoMessage")]
public sealed class OleDbConnection : DbConnection, ICloneable
{
	private string connectionString;

	private int connectionTimeout;

	private IntPtr gdaConnection;

	[DataCategory("Data")]
	[DefaultValue("")]
	[RefreshProperties(RefreshProperties.All)]
	[RecommendedAsConfigurable(true)]
	[Editor("Microsoft.VSDesigner.Data.ADO.Design.OleDbConnectionStringEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public override string ConnectionString
	{
		get
		{
			if (connectionString == null)
			{
				return string.Empty;
			}
			return connectionString;
		}
		set
		{
			connectionString = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override int ConnectionTimeout => connectionTimeout;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override string Database
	{
		get
		{
			if (gdaConnection != IntPtr.Zero && libgda.gda_connection_is_open(gdaConnection))
			{
				return libgda.gda_connection_get_database(gdaConnection);
			}
			return string.Empty;
		}
	}

	[Browsable(true)]
	public override string DataSource
	{
		get
		{
			if (gdaConnection != IntPtr.Zero && libgda.gda_connection_is_open(gdaConnection))
			{
				return libgda.gda_connection_get_dsn(gdaConnection);
			}
			return string.Empty;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(true)]
	public string Provider
	{
		get
		{
			if (gdaConnection != IntPtr.Zero && libgda.gda_connection_is_open(gdaConnection))
			{
				return libgda.gda_connection_get_provider(gdaConnection);
			}
			return string.Empty;
		}
	}

	public override string ServerVersion
	{
		get
		{
			if (State == ConnectionState.Closed)
			{
				throw ExceptionHelper.ConnectionClosed();
			}
			return libgda.gda_connection_get_server_version(gdaConnection);
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override ConnectionState State
	{
		get
		{
			if (gdaConnection != IntPtr.Zero && libgda.gda_connection_is_open(gdaConnection))
			{
				return ConnectionState.Open;
			}
			return ConnectionState.Closed;
		}
	}

	internal IntPtr GdaConnection => gdaConnection;

	[DataCategory("DataCategory_InfoMessage")]
	public event OleDbInfoMessageEventHandler InfoMessage;

	public OleDbConnection()
	{
		gdaConnection = IntPtr.Zero;
		connectionTimeout = 15;
	}

	public OleDbConnection(string connectionString)
		: this()
	{
		this.connectionString = connectionString;
	}

	[System.MonoTODO]
	object ICloneable.Clone()
	{
		throw new NotImplementedException();
	}

	public new OleDbTransaction BeginTransaction()
	{
		if (State == ConnectionState.Closed)
		{
			throw ExceptionHelper.ConnectionClosed();
		}
		return new OleDbTransaction(this);
	}

	public new OleDbTransaction BeginTransaction(IsolationLevel isolationLevel)
	{
		if (State == ConnectionState.Closed)
		{
			throw ExceptionHelper.ConnectionClosed();
		}
		return new OleDbTransaction(this, isolationLevel);
	}

	protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
	{
		return BeginTransaction(isolationLevel);
	}

	protected override DbCommand CreateDbCommand()
	{
		return CreateCommand();
	}

	public override void ChangeDatabase(string value)
	{
		if (State != ConnectionState.Open)
		{
			throw new InvalidOperationException();
		}
		if (!libgda.gda_connection_change_database(gdaConnection, value))
		{
			throw new OleDbException(this);
		}
	}

	public override void Close()
	{
		if (State == ConnectionState.Open)
		{
			libgda.gda_connection_close(gdaConnection);
			gdaConnection = IntPtr.Zero;
		}
	}

	public new OleDbCommand CreateCommand()
	{
		if (State == ConnectionState.Open)
		{
			return new OleDbCommand(null, this);
		}
		return null;
	}

	[System.MonoTODO]
	protected override void Dispose(bool disposing)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public DataTable GetOleDbSchemaTable(Guid schema, object[] restrictions)
	{
		throw new NotImplementedException();
	}

	public override void Open()
	{
		if (State == ConnectionState.Open)
		{
			throw new InvalidOperationException();
		}
		libgda.gda_init("System.Data.OleDb", "1.0", 0, new string[0]);
		gdaConnection = libgda.gda_client_open_connection(libgda.GdaClient, ConnectionString, string.Empty, string.Empty, (GdaConnectionOptions)0);
		if (gdaConnection == IntPtr.Zero)
		{
			throw new OleDbException(this);
		}
	}

	[System.MonoTODO]
	public static void ReleaseObjectPool()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void EnlistDistributedTransaction(ITransaction transaction)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public override void EnlistTransaction(Transaction transaction)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public override DataTable GetSchema()
	{
		if (State == ConnectionState.Closed)
		{
			throw ExceptionHelper.ConnectionClosed();
		}
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public override DataTable GetSchema(string collectionName)
	{
		return GetSchema(collectionName, null);
	}

	[System.MonoTODO]
	public override DataTable GetSchema(string collectionName, string[] restrictionValues)
	{
		if (State == ConnectionState.Closed)
		{
			throw ExceptionHelper.ConnectionClosed();
		}
		throw new NotImplementedException();
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[System.MonoTODO]
	public void ResetState()
	{
		throw new NotImplementedException();
	}
}
