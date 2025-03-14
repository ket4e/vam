using System.Runtime.ConstrainedExecution;

namespace System.Threading;

public class SynchronizationContext
{
	private bool notification_required;

	[ThreadStatic]
	private static SynchronizationContext currentContext;

	public static SynchronizationContext Current => currentContext;

	public SynchronizationContext()
	{
	}

	internal SynchronizationContext(SynchronizationContext context)
	{
		currentContext = context;
	}

	public virtual SynchronizationContext CreateCopy()
	{
		return new SynchronizationContext(this);
	}

	public bool IsWaitNotificationRequired()
	{
		return notification_required;
	}

	public virtual void OperationCompleted()
	{
	}

	public virtual void OperationStarted()
	{
	}

	public virtual void Post(SendOrPostCallback d, object state)
	{
		ThreadPool.QueueUserWorkItem(d.Invoke, state);
	}

	public virtual void Send(SendOrPostCallback d, object state)
	{
		d(state);
	}

	public static void SetSynchronizationContext(SynchronizationContext syncContext)
	{
		currentContext = syncContext;
	}

	[MonoTODO]
	protected void SetWaitNotificationRequired()
	{
		notification_required = true;
		throw new NotImplementedException();
	}

	[CLSCompliant(false)]
	[PrePrepareMethod]
	public virtual int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
	{
		return WaitHelper(waitHandles, waitAll, millisecondsTimeout);
	}

	[CLSCompliant(false)]
	[PrePrepareMethod]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	[MonoTODO]
	protected static int WaitHelper(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
	{
		throw new NotImplementedException();
	}
}
