using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements;

internal class EventCallbackList
{
	private List<EventCallbackFunctorBase> m_List;

	public int capturingCallbackCount { get; private set; }

	public int bubblingCallbackCount { get; private set; }

	public int Count => m_List.Count;

	public EventCallbackFunctorBase this[int i]
	{
		get
		{
			return m_List[i];
		}
		set
		{
			m_List[i] = value;
		}
	}

	public EventCallbackList()
	{
		m_List = new List<EventCallbackFunctorBase>();
		capturingCallbackCount = 0;
		bubblingCallbackCount = 0;
	}

	public EventCallbackList(EventCallbackList source)
	{
		m_List = new List<EventCallbackFunctorBase>(source.m_List);
		capturingCallbackCount = 0;
		bubblingCallbackCount = 0;
	}

	public bool Contains(long eventTypeId, Delegate callback, CallbackPhase phase)
	{
		for (int i = 0; i < m_List.Count; i++)
		{
			if (m_List[i].IsEquivalentTo(eventTypeId, callback, phase))
			{
				return true;
			}
		}
		return false;
	}

	public bool Remove(long eventTypeId, Delegate callback, CallbackPhase phase)
	{
		for (int i = 0; i < m_List.Count; i++)
		{
			if (m_List[i].IsEquivalentTo(eventTypeId, callback, phase))
			{
				m_List.RemoveAt(i);
				switch (phase)
				{
				case CallbackPhase.CaptureAndTarget:
					capturingCallbackCount--;
					break;
				case CallbackPhase.TargetAndBubbleUp:
					bubblingCallbackCount--;
					break;
				}
				return true;
			}
		}
		return false;
	}

	public void Add(EventCallbackFunctorBase item)
	{
		m_List.Add(item);
		if (item.phase == CallbackPhase.CaptureAndTarget)
		{
			capturingCallbackCount++;
		}
		else if (item.phase == CallbackPhase.TargetAndBubbleUp)
		{
			bubblingCallbackCount++;
		}
	}

	public void AddRange(EventCallbackList list)
	{
		m_List.AddRange(list.m_List);
		foreach (EventCallbackFunctorBase item in list.m_List)
		{
			if (item.phase == CallbackPhase.CaptureAndTarget)
			{
				capturingCallbackCount++;
			}
			else if (item.phase == CallbackPhase.TargetAndBubbleUp)
			{
				bubblingCallbackCount++;
			}
		}
	}

	public void Clear()
	{
		m_List.Clear();
		capturingCallbackCount = 0;
		bubblingCallbackCount = 0;
	}
}
