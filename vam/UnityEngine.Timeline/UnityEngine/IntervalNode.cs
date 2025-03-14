using System;
using System.Collections.Generic;

namespace UnityEngine;

internal class IntervalNode<T> where T : class, IInterval
{
	private long m_Center;

	private List<T> m_Children;

	private IntervalNode<T> m_LeftNode;

	private IntervalNode<T> m_RightNode;

	public IntervalNode(ICollection<T> items)
	{
		if (items.Count == 0)
		{
			return;
		}
		long num = long.MaxValue;
		long num2 = long.MinValue;
		foreach (T item in items)
		{
			T current = item;
			num = Math.Min(num, current.intervalStart);
			num2 = Math.Max(num2, current.intervalEnd);
		}
		m_Center = (num2 + num) / 2;
		m_Children = new List<T>();
		List<T> list = new List<T>();
		List<T> list2 = new List<T>();
		foreach (T item2 in items)
		{
			T current2 = item2;
			if (current2.intervalEnd < m_Center)
			{
				list.Add(current2);
			}
			else if (current2.intervalStart > m_Center)
			{
				list2.Add(current2);
			}
			else
			{
				m_Children.Add(current2);
			}
		}
		if (m_Children.Count == 0)
		{
			m_Children = null;
		}
		if (list.Count > 0)
		{
			m_LeftNode = new IntervalNode<T>(list);
		}
		if (list2.Count > 0)
		{
			m_RightNode = new IntervalNode<T>(list2);
		}
	}

	public void Query(long time, int bitflag, ref List<T> results)
	{
		if (m_Children != null)
		{
			foreach (T child in m_Children)
			{
				T current = child;
				if (time >= current.intervalStart && time < current.intervalEnd)
				{
					current.intervalBit = bitflag;
					results.Add(current);
				}
			}
		}
		if (time < m_Center && m_LeftNode != null)
		{
			m_LeftNode.Query(time, bitflag, ref results);
		}
		else if (time > m_Center && m_RightNode != null)
		{
			m_RightNode.Query(time, bitflag, ref results);
		}
	}
}
