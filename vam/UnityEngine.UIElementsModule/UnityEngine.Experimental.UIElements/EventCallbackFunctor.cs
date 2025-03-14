using System;

namespace UnityEngine.Experimental.UIElements;

internal class EventCallbackFunctor<TEventType> : EventCallbackFunctorBase where TEventType : EventBase<TEventType>, new()
{
	private EventCallback<TEventType> m_Callback;

	private long m_EventTypeId;

	public EventCallbackFunctor(EventCallback<TEventType> callback, CallbackPhase phase)
		: base(phase)
	{
		m_Callback = callback;
		m_EventTypeId = EventBase<TEventType>.TypeId();
	}

	public override void Invoke(EventBase evt)
	{
		if (evt == null)
		{
			throw new ArgumentNullException();
		}
		if (evt.GetEventTypeId() == m_EventTypeId && PhaseMatches(evt))
		{
			m_Callback(evt as TEventType);
		}
	}

	public override bool IsEquivalentTo(long eventTypeId, Delegate callback, CallbackPhase phase)
	{
		return m_EventTypeId == eventTypeId && m_Callback == callback && base.phase == phase;
	}
}
internal class EventCallbackFunctor<TEventType, TCallbackArgs> : EventCallbackFunctorBase where TEventType : EventBase<TEventType>, new()
{
	private EventCallback<TEventType, TCallbackArgs> m_Callback;

	private TCallbackArgs m_UserArgs;

	private long m_EventTypeId;

	public EventCallbackFunctor(EventCallback<TEventType, TCallbackArgs> callback, TCallbackArgs userArgs, CallbackPhase phase)
		: base(phase)
	{
		m_Callback = callback;
		m_UserArgs = userArgs;
		m_EventTypeId = EventBase<TEventType>.TypeId();
	}

	public override void Invoke(EventBase evt)
	{
		if (evt == null)
		{
			throw new ArgumentNullException();
		}
		if (evt.GetEventTypeId() == m_EventTypeId && PhaseMatches(evt))
		{
			m_Callback(evt as TEventType, m_UserArgs);
		}
	}

	public override bool IsEquivalentTo(long eventTypeId, Delegate callback, CallbackPhase phase)
	{
		return m_EventTypeId == eventTypeId && m_Callback == callback && base.phase == phase;
	}
}
