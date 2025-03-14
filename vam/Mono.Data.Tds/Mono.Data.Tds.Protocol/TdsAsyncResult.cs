using System;
using System.Threading;

namespace Mono.Data.Tds.Protocol;

internal class TdsAsyncResult : IAsyncResult
{
	private TdsAsyncState _tdsState;

	private WaitHandle _waitHandle;

	private bool _completed;

	private bool _completedSyncly;

	private AsyncCallback _userCallback;

	private object _retValue;

	private Exception _exception;

	public object AsyncState => _tdsState.UserState;

	internal TdsAsyncState TdsAsyncState => _tdsState;

	public WaitHandle AsyncWaitHandle => _waitHandle;

	public bool IsCompleted => _completed;

	public bool IsCompletedWithException => _exception != null;

	public Exception Exception => _exception;

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

	public TdsAsyncResult(AsyncCallback userCallback, TdsAsyncState tdsState)
	{
		_tdsState = tdsState;
		_userCallback = userCallback;
		_waitHandle = new ManualResetEvent(initialState: false);
	}

	public TdsAsyncResult(AsyncCallback userCallback, object state)
	{
		_tdsState = new TdsAsyncState(state);
		_userCallback = userCallback;
		_waitHandle = new ManualResetEvent(initialState: false);
	}

	internal void MarkComplete()
	{
		_completed = true;
		_exception = null;
		((ManualResetEvent)_waitHandle).Set();
		if (_userCallback != null)
		{
			_userCallback(this);
		}
	}

	internal void MarkComplete(Exception e)
	{
		_completed = true;
		_exception = e;
		((ManualResetEvent)_waitHandle).Set();
		if (_userCallback != null)
		{
			_userCallback(this);
		}
	}
}
