using System.Data.Common;
using System.Globalization;

namespace System.Data.Odbc;

public sealed class OdbcTransaction : DbTransaction, IDisposable
{
	private bool disposed;

	private OdbcConnection connection;

	private IsolationLevel isolationlevel;

	private bool isOpen;

	protected override DbConnection DbConnection => Connection;

	public override IsolationLevel IsolationLevel
	{
		get
		{
			if (!isOpen)
			{
				throw ExceptionHelper.TransactionNotUsable(GetType());
			}
			if (isolationlevel == IsolationLevel.Unspecified)
			{
				isolationlevel = GetIsolationLevel(Connection);
			}
			return isolationlevel;
		}
	}

	public new OdbcConnection Connection => connection;

	internal OdbcTransaction(OdbcConnection conn, IsolationLevel isolationlevel)
	{
		SetAutoCommit(conn, isAuto: false);
		OdbcIsolationLevel odbcIsolationLevel = OdbcIsolationLevel.ReadCommitted;
		OdbcConnectionAttribute attribute = OdbcConnectionAttribute.TransactionIsolation;
		switch (isolationlevel)
		{
		case IsolationLevel.ReadUncommitted:
			odbcIsolationLevel = OdbcIsolationLevel.ReadUncommitted;
			break;
		case IsolationLevel.ReadCommitted:
			odbcIsolationLevel = OdbcIsolationLevel.ReadCommitted;
			break;
		case IsolationLevel.RepeatableRead:
			odbcIsolationLevel = OdbcIsolationLevel.RepeatableRead;
			break;
		case IsolationLevel.Serializable:
			odbcIsolationLevel = OdbcIsolationLevel.Serializable;
			break;
		case IsolationLevel.Snapshot:
			odbcIsolationLevel = OdbcIsolationLevel.Snapshot;
			attribute = OdbcConnectionAttribute.CoptTransactionIsolation;
			break;
		case IsolationLevel.Chaos:
			throw new ArgumentOutOfRangeException("IsolationLevel", string.Format(CultureInfo.CurrentCulture, "The IsolationLevel enumeration value, {0}, is not supported by the .Net Framework Odbc Data Provider.", (int)isolationlevel));
		default:
			throw new ArgumentOutOfRangeException("IsolationLevel", string.Format(CultureInfo.CurrentCulture, "The IsolationLevel enumeration value, {0}, is invalid.", (int)isolationlevel));
		case IsolationLevel.Unspecified:
			break;
		}
		if (isolationlevel != IsolationLevel.Unspecified)
		{
			OdbcReturn odbcReturn = libodbc.SQLSetConnectAttr(conn.hDbc, attribute, (IntPtr)(int)odbcIsolationLevel, 0);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw conn.CreateOdbcException(OdbcHandleType.Dbc, conn.hDbc);
			}
		}
		this.isolationlevel = isolationlevel;
		connection = conn;
		isOpen = true;
	}

	void IDisposable.Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private static void SetAutoCommit(OdbcConnection conn, bool isAuto)
	{
		OdbcReturn odbcReturn = libodbc.SQLSetConnectAttr(conn.hDbc, OdbcConnectionAttribute.AutoCommit, (IntPtr)(isAuto ? 1 : 0), -5);
		if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
		{
			throw conn.CreateOdbcException(OdbcHandleType.Dbc, conn.hDbc);
		}
	}

	private static IsolationLevel GetIsolationLevel(OdbcConnection conn)
	{
		int value;
		int StringLength;
		OdbcReturn odbcReturn = libodbc.SQLGetConnectAttr(conn.hDbc, OdbcConnectionAttribute.TransactionIsolation, out value, 0, out StringLength);
		if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
		{
			throw conn.CreateOdbcException(OdbcHandleType.Dbc, conn.hDbc);
		}
		return MapOdbcIsolationLevel((OdbcIsolationLevel)value);
	}

	private static IsolationLevel MapOdbcIsolationLevel(OdbcIsolationLevel odbcLevel)
	{
		IsolationLevel result = IsolationLevel.Unspecified;
		switch (odbcLevel)
		{
		case OdbcIsolationLevel.ReadUncommitted:
			result = IsolationLevel.ReadUncommitted;
			break;
		case OdbcIsolationLevel.ReadCommitted:
			result = IsolationLevel.ReadCommitted;
			break;
		case OdbcIsolationLevel.RepeatableRead:
			result = IsolationLevel.RepeatableRead;
			break;
		case OdbcIsolationLevel.Serializable:
			result = IsolationLevel.Serializable;
			break;
		case OdbcIsolationLevel.Snapshot:
			result = IsolationLevel.Snapshot;
			break;
		}
		return result;
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing && isOpen)
			{
				Rollback();
			}
			disposed = true;
		}
	}

	public override void Commit()
	{
		if (!isOpen)
		{
			throw ExceptionHelper.TransactionNotUsable(GetType());
		}
		if (connection.transaction == this)
		{
			OdbcReturn odbcReturn = libodbc.SQLEndTran(2, connection.hDbc, 0);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw connection.CreateOdbcException(OdbcHandleType.Dbc, connection.hDbc);
			}
			SetAutoCommit(connection, isAuto: true);
			connection.transaction = null;
			connection = null;
			isOpen = false;
			return;
		}
		throw new InvalidOperationException();
	}

	public override void Rollback()
	{
		if (!isOpen)
		{
			throw ExceptionHelper.TransactionNotUsable(GetType());
		}
		if (connection.transaction == this)
		{
			OdbcReturn odbcReturn = libodbc.SQLEndTran(2, connection.hDbc, 1);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw connection.CreateOdbcException(OdbcHandleType.Dbc, connection.hDbc);
			}
			SetAutoCommit(connection, isAuto: true);
			connection.transaction = null;
			connection = null;
			isOpen = false;
			return;
		}
		throw new InvalidOperationException();
	}
}
