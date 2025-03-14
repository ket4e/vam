using System.Threading;

namespace System.Windows.Forms;

internal class AsyncMethodResult : IAsyncResult
{
	private ManualResetEvent handle;

	private object state;

	private bool completed;

	private object return_value;

	private Exception exception;

	public virtual WaitHandle AsyncWaitHandle
	{
		get
		{
			lock (this)
			{
				return handle;
			}
		}
	}

	public object AsyncState
	{
		get
		{
			return state;
		}
		set
		{
			state = value;
		}
	}

	public bool CompletedSynchronously => false;

	public bool IsCompleted
	{
		get
		{
			lock (this)
			{
				return completed;
			}
		}
	}

	public AsyncMethodResult()
	{
		handle = new ManualResetEvent(initialState: false);
	}

	public object EndInvoke()
	{
		lock (this)
		{
			if (completed)
			{
				if (exception == null)
				{
					return return_value;
				}
				throw exception;
			}
		}
		handle.WaitOne();
		if (exception != null)
		{
			throw exception;
		}
		return return_value;
	}

	public void Complete(object result)
	{
		lock (this)
		{
			completed = true;
			return_value = result;
			handle.Set();
		}
	}

	public void CompleteWithException(Exception ex)
	{
		lock (this)
		{
			completed = true;
			exception = ex;
			handle.Set();
		}
	}
}
