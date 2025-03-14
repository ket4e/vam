using System;

namespace UnityEngine.Experimental.UIElements;

internal class EventCallbackRegistry
{
	private static readonly EventCallbackListPool s_ListPool = new EventCallbackListPool();

	private EventCallbackList m_Callbacks;

	private EventCallbackList m_TemporaryCallbacks;

	private int m_IsInvoking;

	public EventCallbackRegistry()
	{
		m_IsInvoking = 0;
	}

	private static EventCallbackList GetCallbackList(EventCallbackList initializer = null)
	{
		return s_ListPool.Get(initializer);
	}

	private static void ReleaseCallbackList(EventCallbackList toRelease)
	{
		s_ListPool.Release(toRelease);
	}

	private EventCallbackList GetCallbackListForWriting()
	{
		if (m_IsInvoking > 0)
		{
			if (m_TemporaryCallbacks == null)
			{
				if (m_Callbacks != null)
				{
					m_TemporaryCallbacks = GetCallbackList(m_Callbacks);
				}
				else
				{
					m_TemporaryCallbacks = GetCallbackList();
				}
			}
			return m_TemporaryCallbacks;
		}
		if (m_Callbacks == null)
		{
			m_Callbacks = GetCallbackList();
		}
		return m_Callbacks;
	}

	private EventCallbackList GetCallbackListForReading()
	{
		if (m_TemporaryCallbacks != null)
		{
			return m_TemporaryCallbacks;
		}
		return m_Callbacks;
	}

	private bool ShouldRegisterCallback(long eventTypeId, Delegate callback, CallbackPhase phase)
	{
		if ((object)callback == null)
		{
			return false;
		}
		EventCallbackList callbackListForReading = GetCallbackListForReading();
		if (callbackListForReading != null)
		{
			return !callbackListForReading.Contains(eventTypeId, callback, phase);
		}
		return true;
	}

	private bool UnregisterCallback(long eventTypeId, Delegate callback, Capture useCapture)
	{
		if ((object)callback == null)
		{
			return false;
		}
		EventCallbackList callbackListForWriting = GetCallbackListForWriting();
		CallbackPhase phase = ((useCapture != Capture.Capture) ? CallbackPhase.TargetAndBubbleUp : CallbackPhase.CaptureAndTarget);
		return callbackListForWriting.Remove(eventTypeId, callback, phase);
	}

	public void RegisterCallback<TEventType>(EventCallback<TEventType> callback, Capture useCapture = Capture.NoCapture) where TEventType : EventBase<TEventType>, new()
	{
		long eventTypeId = EventBase<TEventType>.TypeId();
		CallbackPhase phase = ((useCapture != Capture.Capture) ? CallbackPhase.TargetAndBubbleUp : CallbackPhase.CaptureAndTarget);
		if (ShouldRegisterCallback(eventTypeId, callback, phase))
		{
			EventCallbackList callbackListForWriting = GetCallbackListForWriting();
			callbackListForWriting.Add(new EventCallbackFunctor<TEventType>(callback, phase));
		}
	}

	public void RegisterCallback<TEventType, TCallbackArgs>(EventCallback<TEventType, TCallbackArgs> callback, TCallbackArgs userArgs, Capture useCapture = Capture.NoCapture) where TEventType : EventBase<TEventType>, new()
	{
		long eventTypeId = EventBase<TEventType>.TypeId();
		CallbackPhase phase = ((useCapture != Capture.Capture) ? CallbackPhase.TargetAndBubbleUp : CallbackPhase.CaptureAndTarget);
		if (ShouldRegisterCallback(eventTypeId, callback, phase))
		{
			EventCallbackList callbackListForWriting = GetCallbackListForWriting();
			callbackListForWriting.Add(new EventCallbackFunctor<TEventType, TCallbackArgs>(callback, userArgs, phase));
		}
	}

	public bool UnregisterCallback<TEventType>(EventCallback<TEventType> callback, Capture useCapture = Capture.NoCapture) where TEventType : EventBase<TEventType>, new()
	{
		long eventTypeId = EventBase<TEventType>.TypeId();
		return UnregisterCallback(eventTypeId, callback, useCapture);
	}

	public bool UnregisterCallback<TEventType, TCallbackArgs>(EventCallback<TEventType, TCallbackArgs> callback, Capture useCapture = Capture.NoCapture) where TEventType : EventBase<TEventType>, new()
	{
		long eventTypeId = EventBase<TEventType>.TypeId();
		return UnregisterCallback(eventTypeId, callback, useCapture);
	}

	public void InvokeCallbacks(EventBase evt)
	{
		if (m_Callbacks == null)
		{
			return;
		}
		m_IsInvoking++;
		for (int i = 0; i < m_Callbacks.Count; i++)
		{
			if (evt.isImmediatePropagationStopped)
			{
				break;
			}
			m_Callbacks[i].Invoke(evt);
		}
		m_IsInvoking--;
		if (m_IsInvoking == 0 && m_TemporaryCallbacks != null)
		{
			ReleaseCallbackList(m_Callbacks);
			m_Callbacks = GetCallbackList(m_TemporaryCallbacks);
			ReleaseCallbackList(m_TemporaryCallbacks);
			m_TemporaryCallbacks = null;
		}
	}

	public bool HasCaptureHandlers()
	{
		return m_Callbacks != null && m_Callbacks.capturingCallbackCount > 0;
	}

	public bool HasBubbleHandlers()
	{
		return m_Callbacks != null && m_Callbacks.bubblingCallbackCount > 0;
	}
}
