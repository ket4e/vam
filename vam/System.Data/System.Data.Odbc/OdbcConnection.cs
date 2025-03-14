using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.EnterpriseServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Transactions;

namespace System.Data.Odbc;

[DefaultEvent("InfoMessage")]
public sealed class OdbcConnection : DbConnection, ICloneable
{
	private string connectionString;

	private int connectionTimeout;

	internal OdbcTransaction transaction;

	private IntPtr henv = IntPtr.Zero;

	private IntPtr hdbc = IntPtr.Zero;

	private bool disposed;

	private ArrayList linkedCommands;

	internal IntPtr hDbc => hdbc;

	[RecommendedAsConfigurable(true)]
	[Editor("Microsoft.VSDesigner.Data.Odbc.Design.OdbcConnectionStringEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[RefreshProperties(RefreshProperties.All)]
	[OdbcDescription("Information used to connect to a Data Source")]
	[DefaultValue("")]
	[OdbcCategory("DataCategory_Data")]
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

	[OdbcDescription("Current connection timeout value, not settable  in the ConnectionString")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[DefaultValue(15)]
	public new int ConnectionTimeout
	{
		get
		{
			return connectionTimeout;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentException("Timout should not be less than zero.");
			}
			connectionTimeout = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[OdbcDescription("Current data source Catlog value, 'Database=X' in the ConnectionString")]
	public override string Database
	{
		get
		{
			if (State == ConnectionState.Closed)
			{
				return string.Empty;
			}
			return GetInfo(OdbcInfo.DatabaseName);
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[OdbcDescription("The ConnectionState indicating whether the connection is open or closed")]
	[Browsable(false)]
	public override ConnectionState State
	{
		get
		{
			if (hdbc != IntPtr.Zero)
			{
				return ConnectionState.Open;
			}
			return ConnectionState.Closed;
		}
	}

	[OdbcDescription("Current data source, 'Server=X' in the ConnectionString")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public override string DataSource
	{
		get
		{
			if (State == ConnectionState.Closed)
			{
				return string.Empty;
			}
			return GetInfo(OdbcInfo.DataSourceName);
		}
	}

	[Browsable(false)]
	[OdbcDescription("Current ODBC Driver")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string Driver
	{
		get
		{
			if (State == ConnectionState.Closed)
			{
				return string.Empty;
			}
			return GetInfo(OdbcInfo.DriverName);
		}
	}

	[OdbcDescription("Version of the product accessed by the ODBC Driver")]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override string ServerVersion => GetInfo(OdbcInfo.DbmsVersion);

	internal string SafeDriver
	{
		get
		{
			string safeInfo = GetSafeInfo(OdbcInfo.DriverName);
			if (safeInfo == null)
			{
				return string.Empty;
			}
			return safeInfo;
		}
	}

	[OdbcCategory("DataCategory_InfoMessage")]
	[OdbcDescription("DbConnection_InfoMessage")]
	public event OdbcInfoMessageEventHandler InfoMessage;

	public OdbcConnection()
		: this(string.Empty)
	{
	}

	public OdbcConnection(string connectionString)
	{
		connectionTimeout = 15;
		ConnectionString = connectionString;
	}

	[System.MonoTODO]
	object ICloneable.Clone()
	{
		throw new NotImplementedException();
	}

	public new OdbcTransaction BeginTransaction()
	{
		return BeginTransaction(IsolationLevel.Unspecified);
	}

	protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
	{
		return BeginTransaction(isolationLevel);
	}

	public new OdbcTransaction BeginTransaction(IsolationLevel isolevel)
	{
		if (State == ConnectionState.Closed)
		{
			throw ExceptionHelper.ConnectionClosed();
		}
		if (transaction == null)
		{
			transaction = new OdbcTransaction(this, isolevel);
			return transaction;
		}
		throw new InvalidOperationException();
	}

	public override void Close()
	{
		OdbcReturn odbcReturn = OdbcReturn.Error;
		if (State != ConnectionState.Open)
		{
			return;
		}
		if (linkedCommands != null)
		{
			for (int i = 0; i < linkedCommands.Count; i++)
			{
				WeakReference weakReference = (WeakReference)linkedCommands[i];
				if (weakReference != null)
				{
					((OdbcCommand)weakReference.Target)?.Unlink();
				}
			}
			linkedCommands = null;
		}
		odbcReturn = libodbc.SQLDisconnect(hdbc);
		if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
		{
			throw CreateOdbcException(OdbcHandleType.Dbc, hdbc);
		}
		FreeHandles();
		transaction = null;
		RaiseStateChange(ConnectionState.Open, ConnectionState.Closed);
	}

	public new OdbcCommand CreateCommand()
	{
		return new OdbcCommand(string.Empty, this, transaction);
	}

	public override void ChangeDatabase(string value)
	{
		IntPtr intPtr = IntPtr.Zero;
		OdbcReturn odbcReturn = OdbcReturn.Error;
		try
		{
			intPtr = Marshal.StringToHGlobalUni(value);
			odbcReturn = libodbc.SQLSetConnectAttr(hdbc, OdbcConnectionAttribute.CurrentCatalog, intPtr, value.Length * 2);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw CreateOdbcException(OdbcHandleType.Dbc, hdbc);
			}
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeCoTaskMem(intPtr);
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposed)
		{
			try
			{
				Close();
				disposed = true;
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
	}

	protected override DbCommand CreateDbCommand()
	{
		return CreateCommand();
	}

	public override void Open()
	{
		if (State == ConnectionState.Open)
		{
			throw new InvalidOperationException();
		}
		OdbcReturn odbcReturn = OdbcReturn.Error;
		OdbcException ex = null;
		try
		{
			odbcReturn = libodbc.SQLAllocHandle(OdbcHandleType.Env, IntPtr.Zero, ref henv);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				OdbcErrorCollection odbcErrorCollection = new OdbcErrorCollection();
				odbcErrorCollection.Add(new OdbcError(this));
				ex = new OdbcException(odbcErrorCollection);
				MessageHandler(ex);
				throw ex;
			}
			odbcReturn = libodbc.SQLSetEnvAttr(henv, OdbcEnv.OdbcVersion, (IntPtr)3, 0);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw CreateOdbcException(OdbcHandleType.Env, henv);
			}
			odbcReturn = libodbc.SQLAllocHandle(OdbcHandleType.Dbc, henv, ref hdbc);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw CreateOdbcException(OdbcHandleType.Env, henv);
			}
			if (ConnectionString.ToLower().IndexOf("dsn=") >= 0)
			{
				string userName = string.Empty;
				string authentication = string.Empty;
				string serverName = string.Empty;
				string[] array = ConnectionString.Split(';');
				string[] array2 = array;
				foreach (string text in array2)
				{
					string[] array3 = text.Split('=');
					switch (array3[0].Trim().ToLower())
					{
					case "dsn":
						serverName = array3[1].Trim();
						break;
					case "uid":
						userName = array3[1].Trim();
						break;
					case "pwd":
						authentication = array3[1].Trim();
						break;
					}
				}
				odbcReturn = libodbc.SQLConnect(hdbc, serverName, -3, userName, -3, authentication, -3);
				if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw CreateOdbcException(OdbcHandleType.Dbc, hdbc);
				}
			}
			else
			{
				string text2 = new string(' ', 1024);
				short StringLength2Ptr = 0;
				odbcReturn = libodbc.SQLDriverConnect(hdbc, IntPtr.Zero, ConnectionString, -3, text2, (short)text2.Length, ref StringLength2Ptr, 0);
				if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw CreateOdbcException(OdbcHandleType.Dbc, hdbc);
				}
			}
			RaiseStateChange(ConnectionState.Closed, ConnectionState.Open);
		}
		catch
		{
			FreeHandles();
			throw;
		}
		disposed = false;
	}

	[System.MonoTODO]
	public static void ReleaseObjectPool()
	{
		throw new NotImplementedException();
	}

	private void FreeHandles()
	{
		OdbcReturn odbcReturn = OdbcReturn.Error;
		if (hdbc != IntPtr.Zero)
		{
			odbcReturn = libodbc.SQLFreeHandle(2, hdbc);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw CreateOdbcException(OdbcHandleType.Dbc, hdbc);
			}
		}
		hdbc = IntPtr.Zero;
		if (henv != IntPtr.Zero)
		{
			odbcReturn = libodbc.SQLFreeHandle(1, henv);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw CreateOdbcException(OdbcHandleType.Env, henv);
			}
		}
		henv = IntPtr.Zero;
	}

	public override DataTable GetSchema()
	{
		if (State == ConnectionState.Closed)
		{
			throw ExceptionHelper.ConnectionClosed();
		}
		return MetaDataCollections.Instance;
	}

	public override DataTable GetSchema(string collectionName)
	{
		return GetSchema(collectionName, null);
	}

	public override DataTable GetSchema(string collectionName, string[] restrictionValues)
	{
		if (State == ConnectionState.Closed)
		{
			throw ExceptionHelper.ConnectionClosed();
		}
		return GetSchema(collectionName, null);
	}

	[System.MonoTODO]
	public override void EnlistTransaction(Transaction transaction)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void EnlistDistributedTransaction(ITransaction transaction)
	{
		throw new NotImplementedException();
	}

	internal string GetInfo(OdbcInfo info)
	{
		if (State == ConnectionState.Closed)
		{
			throw new InvalidOperationException("The connection is closed.");
		}
		OdbcReturn odbcReturn = OdbcReturn.Error;
		short buffLength = 512;
		byte[] array = new byte[512];
		short remainingStrLen = 0;
		odbcReturn = libodbc.SQLGetInfo(hdbc, info, array, buffLength, ref remainingStrLen);
		if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
		{
			throw CreateOdbcException(OdbcHandleType.Dbc, hdbc);
		}
		return Encoding.Unicode.GetString(array, 0, remainingStrLen);
	}

	private string GetSafeInfo(OdbcInfo info)
	{
		if (State == ConnectionState.Closed)
		{
			return null;
		}
		OdbcReturn odbcReturn = OdbcReturn.Error;
		short buffLength = 512;
		byte[] array = new byte[512];
		short remainingStrLen = 0;
		odbcReturn = libodbc.SQLGetInfo(hdbc, info, array, buffLength, ref remainingStrLen);
		if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
		{
			return null;
		}
		return Encoding.Unicode.GetString(array, 0, remainingStrLen);
	}

	private void RaiseStateChange(ConnectionState from, ConnectionState to)
	{
		base.OnStateChange(new StateChangeEventArgs(from, to));
	}

	private OdbcInfoMessageEventArgs CreateOdbcInfoMessageEvent(OdbcErrorCollection errors)
	{
		return new OdbcInfoMessageEventArgs(errors);
	}

	private void OnOdbcInfoMessage(OdbcInfoMessageEventArgs e)
	{
		if (this.InfoMessage != null)
		{
			this.InfoMessage(this, e);
		}
	}

	internal OdbcException CreateOdbcException(OdbcHandleType HandleType, IntPtr Handle)
	{
		short num = 256;
		short TextLength = 0;
		int NativeError = 0;
		OdbcReturn odbcReturn = OdbcReturn.Success;
		OdbcErrorCollection odbcErrorCollection = new OdbcErrorCollection();
		while (true)
		{
			byte[] array = new byte[num * 2];
			byte[] array2 = new byte[num * 2];
			switch (HandleType)
			{
			case OdbcHandleType.Dbc:
				odbcReturn = libodbc.SQLError(IntPtr.Zero, Handle, IntPtr.Zero, array2, ref NativeError, array, num, ref TextLength);
				break;
			case OdbcHandleType.Stmt:
				odbcReturn = libodbc.SQLError(IntPtr.Zero, IntPtr.Zero, Handle, array2, ref NativeError, array, num, ref TextLength);
				break;
			case OdbcHandleType.Env:
				odbcReturn = libodbc.SQLError(Handle, IntPtr.Zero, IntPtr.Zero, array2, ref NativeError, array, num, ref TextLength);
				break;
			}
			if (odbcReturn != 0)
			{
				break;
			}
			string state = RemoveTrailingNullChar(Encoding.Unicode.GetString(array2));
			string @string = Encoding.Unicode.GetString(array, 0, TextLength * 2);
			odbcErrorCollection.Add(new OdbcError(@string, state, NativeError));
		}
		string safeDriver = SafeDriver;
		foreach (OdbcError item in odbcErrorCollection)
		{
			item.SetSource(safeDriver);
		}
		return new OdbcException(odbcErrorCollection);
	}

	private static string RemoveTrailingNullChar(string value)
	{
		return value.TrimEnd(default(char));
	}

	internal void Link(OdbcCommand cmd)
	{
		if (linkedCommands == null)
		{
			linkedCommands = new ArrayList();
		}
		linkedCommands.Add(new WeakReference(cmd));
	}

	internal void Unlink(OdbcCommand cmd)
	{
		if (linkedCommands == null)
		{
			return;
		}
		for (int i = 0; i < linkedCommands.Count; i++)
		{
			WeakReference weakReference = (WeakReference)linkedCommands[i];
			if (weakReference != null)
			{
				OdbcCommand odbcCommand = (OdbcCommand)weakReference.Target;
				if (odbcCommand == cmd)
				{
					linkedCommands[i] = null;
					break;
				}
			}
		}
	}

	private void MessageHandler(OdbcException e)
	{
		OnOdbcInfoMessage(CreateOdbcInfoMessageEvent(e.Errors));
	}
}
