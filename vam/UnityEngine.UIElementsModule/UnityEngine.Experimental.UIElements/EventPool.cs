using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements;

internal class EventPool<T> where T : EventBase<T>, new()
{
	private readonly Stack<T> m_Stack = new Stack<T>();

	public T Get()
	{
		return (m_Stack.Count != 0) ? m_Stack.Pop() : new T();
	}

	public void Release(T element)
	{
		if (m_Stack.Contains(element))
		{
			Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
		}
		m_Stack.Push(element);
	}
}
