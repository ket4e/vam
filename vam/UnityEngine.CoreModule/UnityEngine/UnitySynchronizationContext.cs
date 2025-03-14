using System.Collections.Generic;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityEngine;

internal sealed class UnitySynchronizationContext : SynchronizationContext
{
	private struct WorkRequest
	{
		private readonly SendOrPostCallback m_DelagateCallback;

		private readonly object m_DelagateState;

		private readonly ManualResetEvent m_WaitHandle;

		public WorkRequest(SendOrPostCallback callback, object state, ManualResetEvent waitHandle = null)
		{
			m_DelagateCallback = callback;
			m_DelagateState = state;
			m_WaitHandle = waitHandle;
		}

		public void Invoke()
		{
			m_DelagateCallback(m_DelagateState);
			if (m_WaitHandle != null)
			{
				m_WaitHandle.Set();
			}
		}
	}

	private const int kAwqInitialCapacity = 20;

	private readonly Queue<WorkRequest> m_AsyncWorkQueue = new Queue<WorkRequest>(20);

	private readonly int m_MainThreadID = Thread.CurrentThread.ManagedThreadId;

	public override void Send(SendOrPostCallback callback, object state)
	{
		if (m_MainThreadID == Thread.CurrentThread.ManagedThreadId)
		{
			callback(state);
			return;
		}
		using ManualResetEvent manualResetEvent = new ManualResetEvent(initialState: false);
		lock (m_AsyncWorkQueue)
		{
			m_AsyncWorkQueue.Enqueue(new WorkRequest(callback, state, manualResetEvent));
		}
		manualResetEvent.WaitOne();
	}

	public override void Post(SendOrPostCallback callback, object state)
	{
		lock (m_AsyncWorkQueue)
		{
			m_AsyncWorkQueue.Enqueue(new WorkRequest(callback, state));
		}
	}

	private void Exec()
	{
		lock (m_AsyncWorkQueue)
		{
			int count = m_AsyncWorkQueue.Count;
			for (int i = 0; i < count; i++)
			{
				m_AsyncWorkQueue.Dequeue().Invoke();
			}
		}
	}

	[RequiredByNativeCode]
	private static void InitializeSynchronizationContext()
	{
		if (SynchronizationContext.Current == null)
		{
			SynchronizationContext.SetSynchronizationContext(new UnitySynchronizationContext());
		}
	}

	[RequiredByNativeCode]
	private static void ExecuteTasks()
	{
		if (SynchronizationContext.Current is UnitySynchronizationContext unitySynchronizationContext)
		{
			unitySynchronizationContext.Exec();
		}
	}
}
