using System.Threading;

namespace System.Data.SqlClient;

internal class SqlAsyncResult : IAsyncResult
{
	private SqlAsyncState _sqlState;

	private WaitHandle _waitHandle;

	private bool _completed;

	private bool _completedSyncly;

	private bool _ended;

	private AsyncCallback _userCallback;

	private object _retValue;

	private string _endMethod;

	private IAsyncResult _internal;

	public object AsyncState => _sqlState.UserState;

	internal SqlAsyncState SqlAsyncState => _sqlState;

	public WaitHandle AsyncWaitHandle => _waitHandle;

	public bool IsCompleted => _completed;

	public bool CompletedSynchronously => _completedSyncly;

	internal object ReturnValue
	{
		get
		{
			return _retValue;
		}
		set
		{
			_retValue = value;
		}
	}

	public string EndMethod
	{
		get
		{
			return _endMethod;
		}
		set
		{
			_endMethod = value;
		}
	}

	public bool Ended
	{
		get
		{
			return _ended;
		}
		set
		{
			_ended = value;
		}
	}

	internal IAsyncResult InternalResult
	{
		get
		{
			return _internal;
		}
		set
		{
			_internal = value;
		}
	}

	public AsyncCallback BubbleCallback => Bubbleback;

	public SqlAsyncResult(AsyncCallback userCallback, SqlAsyncState sqlState)
	{
		_sqlState = sqlState;
		_userCallback = userCallback;
		_waitHandle = new ManualResetEvent(initialState: false);
	}

	public SqlAsyncResult(AsyncCallback userCallback, object state)
	{
		_sqlState = new SqlAsyncState(state);
		_userCallback = userCallback;
		_waitHandle = new ManualResetEvent(initialState: false);
	}

	internal void MarkComplete()
	{
		_completed = true;
		((ManualResetEvent)_waitHandle).Set();
		if (_userCallback != null)
		{
			_userCallback(this);
		}
	}

	public void Bubbleback(IAsyncResult ar)
	{
		MarkComplete();
	}
}
