using System.Data.Common;

namespace System.Data.OleDb;

public sealed class OleDbTransaction : DbTransaction, IDisposable, IDbTransaction
{
	private bool disposed;

	private OleDbConnection connection;

	private IntPtr gdaTransaction;

	private int depth;

	private bool isOpen;

	public new OleDbConnection Connection => connection;

	protected override DbConnection DbConnection => connection;

	public override IsolationLevel IsolationLevel
	{
		get
		{
			if (!isOpen)
			{
				throw ExceptionHelper.TransactionNotUsable(GetType());
			}
			return libgda.gda_transaction_get_isolation_level(gdaTransaction) switch
			{
				GdaTransactionIsolation.ReadCommitted => IsolationLevel.ReadCommitted, 
				GdaTransactionIsolation.ReadUncommitted => IsolationLevel.ReadUncommitted, 
				GdaTransactionIsolation.RepeatableRead => IsolationLevel.RepeatableRead, 
				GdaTransactionIsolation.Serializable => IsolationLevel.Serializable, 
				_ => IsolationLevel.Unspecified, 
			};
		}
	}

	internal OleDbTransaction(OleDbConnection connection, int depth)
		: this(connection, depth, IsolationLevel.ReadCommitted)
	{
	}

	internal OleDbTransaction(OleDbConnection connection)
		: this(connection, 1)
	{
	}

	internal OleDbTransaction(OleDbConnection connection, int depth, IsolationLevel isolevel)
	{
		this.connection = connection;
		gdaTransaction = libgda.gda_transaction_new(depth.ToString());
		switch (isolevel)
		{
		case IsolationLevel.ReadCommitted:
			libgda.gda_transaction_set_isolation_level(gdaTransaction, GdaTransactionIsolation.ReadCommitted);
			break;
		case IsolationLevel.ReadUncommitted:
			libgda.gda_transaction_set_isolation_level(gdaTransaction, GdaTransactionIsolation.ReadUncommitted);
			break;
		case IsolationLevel.RepeatableRead:
			libgda.gda_transaction_set_isolation_level(gdaTransaction, GdaTransactionIsolation.RepeatableRead);
			break;
		case IsolationLevel.Serializable:
			libgda.gda_transaction_set_isolation_level(gdaTransaction, GdaTransactionIsolation.Serializable);
			break;
		}
		libgda.gda_connection_begin_transaction(connection.GdaConnection, gdaTransaction);
		isOpen = true;
	}

	internal OleDbTransaction(OleDbConnection connection, IsolationLevel isolevel)
		: this(connection, 1, isolevel)
	{
	}

	public OleDbTransaction Begin()
	{
		if (!isOpen)
		{
			throw ExceptionHelper.TransactionNotUsable(GetType());
		}
		return new OleDbTransaction(connection, depth + 1);
	}

	public OleDbTransaction Begin(IsolationLevel isolevel)
	{
		if (!isOpen)
		{
			throw ExceptionHelper.TransactionNotUsable(GetType());
		}
		return new OleDbTransaction(connection, depth + 1, isolevel);
	}

	public override void Commit()
	{
		if (!isOpen)
		{
			throw ExceptionHelper.TransactionNotUsable(GetType());
		}
		if (!libgda.gda_connection_commit_transaction(connection.GdaConnection, gdaTransaction))
		{
			throw new InvalidOperationException();
		}
		connection = null;
		isOpen = false;
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
		base.Dispose(disposing);
	}

	public override void Rollback()
	{
		if (!isOpen)
		{
			throw ExceptionHelper.TransactionNotUsable(GetType());
		}
		if (!libgda.gda_connection_rollback_transaction(connection.GdaConnection, gdaTransaction))
		{
			throw new InvalidOperationException();
		}
		connection = null;
		isOpen = false;
	}
}
