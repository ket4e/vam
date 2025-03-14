using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Implementation of a linear focus ring. Elements are sorted according to their focusIndex.</para>
/// </summary>
public class VisualElementFocusRing : IFocusRing
{
	/// <summary>
	///   <para>Ordering of elements in the focus ring.</para>
	/// </summary>
	public enum DefaultFocusOrder
	{
		/// <summary>
		///   <para>Order elements using a depth-first pre-order traversal of the element tree.</para>
		/// </summary>
		ChildOrder,
		/// <summary>
		///   <para>Order elements according to their position, first by X, then by Y.</para>
		/// </summary>
		PositionXY,
		/// <summary>
		///   <para>Order elements according to their position, first by Y, then by X.</para>
		/// </summary>
		PositionYX
	}

	private struct FocusRingRecord
	{
		public int m_AutoIndex;

		public Focusable m_Focusable;
	}

	private VisualElement root;

	private List<FocusRingRecord> m_FocusRing;

	/// <summary>
	///   <para>The focus order for elements having 0 has a focusIndex.</para>
	/// </summary>
	public DefaultFocusOrder defaultFocusOrder { get; set; }

	public VisualElementFocusRing(VisualElement root, DefaultFocusOrder dfo = DefaultFocusOrder.ChildOrder)
	{
		defaultFocusOrder = dfo;
		this.root = root;
		m_FocusRing = new List<FocusRingRecord>();
	}

	private int FocusRingSort(FocusRingRecord a, FocusRingRecord b)
	{
		if (a.m_Focusable.focusIndex == 0 && b.m_Focusable.focusIndex == 0)
		{
			switch (defaultFocusOrder)
			{
			default:
				return Comparer<int>.Default.Compare(a.m_AutoIndex, b.m_AutoIndex);
			case DefaultFocusOrder.PositionXY:
			{
				VisualElement visualElement3 = a.m_Focusable as VisualElement;
				VisualElement visualElement4 = b.m_Focusable as VisualElement;
				if (visualElement3 != null && visualElement4 != null)
				{
					if (visualElement3.layout.position.x < visualElement4.layout.position.x)
					{
						return -1;
					}
					if (visualElement3.layout.position.x > visualElement4.layout.position.x)
					{
						return 1;
					}
					if (visualElement3.layout.position.y < visualElement4.layout.position.y)
					{
						return -1;
					}
					if (visualElement3.layout.position.y > visualElement4.layout.position.y)
					{
						return 1;
					}
				}
				return Comparer<int>.Default.Compare(a.m_AutoIndex, b.m_AutoIndex);
			}
			case DefaultFocusOrder.PositionYX:
			{
				VisualElement visualElement = a.m_Focusable as VisualElement;
				VisualElement visualElement2 = b.m_Focusable as VisualElement;
				if (visualElement != null && visualElement2 != null)
				{
					if (visualElement.layout.position.y < visualElement2.layout.position.y)
					{
						return -1;
					}
					if (visualElement.layout.position.y > visualElement2.layout.position.y)
					{
						return 1;
					}
					if (visualElement.layout.position.x < visualElement2.layout.position.x)
					{
						return -1;
					}
					if (visualElement.layout.position.x > visualElement2.layout.position.x)
					{
						return 1;
					}
				}
				return Comparer<int>.Default.Compare(a.m_AutoIndex, b.m_AutoIndex);
			}
			}
		}
		if (a.m_Focusable.focusIndex == 0)
		{
			return 1;
		}
		if (b.m_Focusable.focusIndex == 0)
		{
			return -1;
		}
		return Comparer<int>.Default.Compare(a.m_Focusable.focusIndex, b.m_Focusable.focusIndex);
	}

	private void DoUpdate()
	{
		m_FocusRing.Clear();
		if (root != null)
		{
			int focusIndex = 0;
			BuildRingRecursive(root, ref focusIndex);
			m_FocusRing.Sort(FocusRingSort);
		}
	}

	private void BuildRingRecursive(VisualElement vc, ref int focusIndex)
	{
		for (int i = 0; i < vc.shadow.childCount; i++)
		{
			VisualElement visualElement = vc.shadow[i];
			if (visualElement.canGrabFocus)
			{
				m_FocusRing.Add(new FocusRingRecord
				{
					m_AutoIndex = focusIndex++,
					m_Focusable = visualElement
				});
			}
			BuildRingRecursive(visualElement, ref focusIndex);
		}
	}

	private int GetFocusableInternalIndex(Focusable f)
	{
		if (f != null)
		{
			for (int i = 0; i < m_FocusRing.Count; i++)
			{
				if (f == m_FocusRing[i].m_Focusable)
				{
					return i;
				}
			}
		}
		return -1;
	}

	/// <summary>
	///   <para>Get the direction of the focus change for the given event. For example, when the Tab key is pressed, focus should be given to the element to the right in the focus ring.</para>
	/// </summary>
	/// <param name="currentFocusable"></param>
	/// <param name="e"></param>
	public FocusChangeDirection GetFocusChangeDirection(Focusable currentFocusable, EventBase e)
	{
		if (currentFocusable is IMGUIContainer && e.imguiEvent != null)
		{
			return FocusChangeDirection.none;
		}
		if (e.GetEventTypeId() == EventBase<KeyDownEvent>.TypeId())
		{
			KeyDownEvent keyDownEvent = e as KeyDownEvent;
			EventModifiers modifiers = keyDownEvent.modifiers;
			if (keyDownEvent.character == '\t')
			{
				if (currentFocusable == null)
				{
					return FocusChangeDirection.none;
				}
				if ((modifiers & EventModifiers.Shift) == 0)
				{
					return VisualElementFocusChangeDirection.right;
				}
				return VisualElementFocusChangeDirection.left;
			}
		}
		return FocusChangeDirection.none;
	}

	/// <summary>
	///   <para>Get the next element in the given direction.</para>
	/// </summary>
	/// <param name="currentFocusable"></param>
	/// <param name="direction"></param>
	public Focusable GetNextFocusable(Focusable currentFocusable, FocusChangeDirection direction)
	{
		if (direction == FocusChangeDirection.none || direction == FocusChangeDirection.unspecified)
		{
			return currentFocusable;
		}
		DoUpdate();
		if (m_FocusRing.Count == 0)
		{
			return null;
		}
		int num = 0;
		if (direction == VisualElementFocusChangeDirection.right)
		{
			num = GetFocusableInternalIndex(currentFocusable) + 1;
			if (num == m_FocusRing.Count)
			{
				num = 0;
			}
		}
		else if (direction == VisualElementFocusChangeDirection.left)
		{
			num = GetFocusableInternalIndex(currentFocusable) - 1;
			if (num == -1)
			{
				num = m_FocusRing.Count - 1;
			}
		}
		return m_FocusRing[num].m_Focusable;
	}
}
