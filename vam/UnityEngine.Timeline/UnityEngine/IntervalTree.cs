using System.Collections.Generic;

namespace UnityEngine;

internal class IntervalTree<T> where T : class, IInterval
{
	private List<T> m_Nodes = new List<T>();

	private bool m_Dirty = true;

	private IntervalNode<T> m_Root;

	public void Add(T item)
	{
		m_Nodes.Add(item);
		m_Dirty = true;
	}

	public void IntersectsWith(long value, int bitFlag, ref List<T> results)
	{
		if (m_Dirty)
		{
			m_Root = new IntervalNode<T>(m_Nodes);
			m_Dirty = false;
		}
		m_Root.Query(value, bitFlag, ref results);
	}
}
