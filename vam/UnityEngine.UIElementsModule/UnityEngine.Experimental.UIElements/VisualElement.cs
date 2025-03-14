#define UNITY_ASSERTIONS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.CSSLayout;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.Experimental.UIElements.StyleSheets;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Base class for objects that are part of the UIElements visual tree.</para>
/// </summary>
public class VisualElement : Focusable, ITransform, IUIElementDataWatch, IEnumerable<VisualElement>, IVisualElementScheduler, IStyle, IEnumerable
{
	/// <summary>
	///   <para>The modes available to measure VisualElement sizes.</para>
	/// </summary>
	public enum MeasureMode
	{
		/// <summary>
		///   <para>The element should give its preferred width/height without any constraint.</para>
		/// </summary>
		Undefined,
		/// <summary>
		///   <para>The element should give the width/height that is passed in and derive the opposite site from this value (for example, calculate text size from a fixed width).</para>
		/// </summary>
		Exactly,
		/// <summary>
		///   <para>At Most. The element should give its preferred width/height but no more than the value passed.</para>
		/// </summary>
		AtMost
	}

	private class DataWatchRequest : IUIElementDataWatchRequest, IVisualElementPanelActivatable, IDisposable
	{
		private VisualElementPanelActivator m_Activator;

		public Action<Object> notification { get; set; }

		public Object watchedObject { get; set; }

		public IDataWatchHandle requestedHandle { get; set; }

		public VisualElement element { get; set; }

		public DataWatchRequest(VisualElement handler)
		{
			element = handler;
			m_Activator = new VisualElementPanelActivator(this);
		}

		public void Start()
		{
			m_Activator.SetActive(action: true);
		}

		public void Stop()
		{
			m_Activator.SetActive(action: false);
		}

		public bool CanBeActivated()
		{
			return element != null && element.elementPanel != null && element.elementPanel.dataWatch != null;
		}

		public void OnPanelActivate()
		{
			if (requestedHandle == null)
			{
				requestedHandle = element.elementPanel.dataWatch.AddWatch(watchedObject, notification);
			}
		}

		public void OnPanelDeactivate()
		{
			if (requestedHandle != null)
			{
				element.elementPanel.dataWatch.RemoveWatch(requestedHandle);
				requestedHandle = null;
			}
		}

		public void Dispose()
		{
			Stop();
		}
	}

	/// <summary>
	///   <para>Options to select clipping strategy.</para>
	/// </summary>
	public enum ClippingOptions
	{
		/// <summary>
		///   <para>Will enable clipping. This VisualElement and its children's content will be limited to this element's bounds.</para>
		/// </summary>
		ClipContents,
		/// <summary>
		///   <para>Will disable clipping and let children VisualElements paint outside its bounds.</para>
		/// </summary>
		NoClipping,
		/// <summary>
		///   <para>Enables clipping and renders contents to a cache texture.</para>
		/// </summary>
		ClipAndCacheContents
	}

	/// <summary>
	///   <para>Hierarchy is a sctuct allowing access to the shadow hierarchy of visual elements</para>
	/// </summary>
	public struct Hierarchy
	{
		private readonly VisualElement m_Owner;

		/// <summary>
		///   <para> Access the physical parent of this element in the hierarchy
		///           </para>
		/// </summary>
		public VisualElement parent => m_Owner.m_PhysicalParent;

		/// <summary>
		///   <para> Number of child elements in this object's contentContainer
		///           </para>
		/// </summary>
		public int childCount => (m_Owner.m_Children != null) ? m_Owner.m_Children.Count : 0;

		public VisualElement this[int key] => ElementAt(key);

		internal Hierarchy(VisualElement element)
		{
			m_Owner = element;
		}

		/// <summary>
		///   <para>Add an element to this element's contentContainer</para>
		/// </summary>
		/// <param name="child"></param>
		public void Add(VisualElement child)
		{
			if (child == null)
			{
				throw new ArgumentException("Cannot add null child");
			}
			Insert(childCount, child);
		}

		public void Insert(int index, VisualElement child)
		{
			if (child == null)
			{
				throw new ArgumentException("Cannot insert null child");
			}
			if (index > childCount)
			{
				throw new IndexOutOfRangeException("Index out of range: " + index);
			}
			if (child == m_Owner)
			{
				throw new ArgumentException("Cannot insert element as its own child");
			}
			child.RemoveFromHierarchy();
			child.shadow.SetParent(m_Owner);
			if (m_Owner.m_Children == null)
			{
				m_Owner.m_Children = new List<VisualElement>();
			}
			if (m_Owner.cssNode.IsMeasureDefined)
			{
				m_Owner.cssNode.SetMeasureFunction(null);
			}
			PutChildAtIndex(child, index);
			child.SetEnabledFromHierarchy(m_Owner.enabledInHierarchy);
			child.Dirty(ChangeType.Styles);
			child.Dirty(ChangeType.Transform);
			m_Owner.Dirty(ChangeType.Layout);
			if (!string.IsNullOrEmpty(child.persistenceKey))
			{
				child.Dirty(ChangeType.PersistentData);
			}
		}

		/// <summary>
		///   <para>Removes this child from the hierarchy</para>
		/// </summary>
		/// <param name="child"></param>
		public void Remove(VisualElement child)
		{
			if (child == null)
			{
				throw new ArgumentException("Cannot remove null child");
			}
			if (child.shadow.parent != m_Owner)
			{
				throw new ArgumentException("This visualElement is not my child");
			}
			if (m_Owner.m_Children != null)
			{
				int index = m_Owner.m_Children.IndexOf(child);
				RemoveAt(index);
			}
		}

		/// <summary>
		///   <para>Remove the child element located at this position from this element's contentContainer</para>
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			if (index < 0 || index >= childCount)
			{
				throw new IndexOutOfRangeException("Index out of range: " + index);
			}
			VisualElement visualElement = m_Owner.m_Children[index];
			visualElement.shadow.SetParent(null);
			RemoveChildAtIndex(index);
			if (childCount == 0)
			{
				m_Owner.cssNode.SetMeasureFunction(m_Owner.Measure);
			}
			m_Owner.Dirty(ChangeType.Layout);
		}

		/// <summary>
		///   <para>Remove all child elements from this element's contentContainer</para>
		/// </summary>
		public void Clear()
		{
			if (childCount <= 0)
			{
				return;
			}
			foreach (VisualElement child in m_Owner.m_Children)
			{
				child.shadow.SetParent(null);
				child.m_LogicalParent = null;
			}
			m_Owner.m_Children.Clear();
			m_Owner.cssNode.Clear();
			m_Owner.Dirty(ChangeType.Layout);
		}

		internal void BringToFront(VisualElement child)
		{
			if (childCount > 1)
			{
				int num = m_Owner.m_Children.IndexOf(child);
				if (num >= 0 && num < childCount - 1)
				{
					RemoveChildAtIndex(num);
					PutChildAtIndex(child, childCount);
					m_Owner.Dirty(ChangeType.Layout);
				}
			}
		}

		internal void SendToBack(VisualElement child)
		{
			if (childCount > 1)
			{
				int num = m_Owner.m_Children.IndexOf(child);
				if (num > 0)
				{
					RemoveChildAtIndex(num);
					PutChildAtIndex(child, 0);
					m_Owner.Dirty(ChangeType.Layout);
				}
			}
		}

		internal void PlaceBehind(VisualElement child, VisualElement over)
		{
			if (childCount <= 0)
			{
				return;
			}
			int num = m_Owner.m_Children.IndexOf(child);
			if (num >= 0)
			{
				RemoveChildAtIndex(num);
				num = m_Owner.m_Children.IndexOf(over);
				if (num < 0)
				{
					num = 0;
				}
				PutChildAtIndex(child, num);
				m_Owner.Dirty(ChangeType.Layout);
			}
		}

		internal void PlaceInFront(VisualElement child, VisualElement under)
		{
			if (childCount > 0)
			{
				int num = m_Owner.m_Children.IndexOf(child);
				if (num >= 0)
				{
					RemoveChildAtIndex(num);
					num = m_Owner.m_Children.IndexOf(under) + 1;
					PutChildAtIndex(child, num);
					m_Owner.Dirty(ChangeType.Layout);
				}
			}
		}

		/// <summary>
		///   <para>Retrieves the child element at position</para>
		/// </summary>
		/// <param name="index"></param>
		public VisualElement ElementAt(int index)
		{
			if (m_Owner.m_Children != null)
			{
				return m_Owner.m_Children[index];
			}
			throw new IndexOutOfRangeException("Index out of range: " + index);
		}

		/// <summary>
		///   <para>Returns the elements from its contentContainer</para>
		/// </summary>
		public IEnumerable<VisualElement> Children()
		{
			if (m_Owner.m_Children != null)
			{
				return m_Owner.m_Children;
			}
			return s_EmptyList;
		}

		private void SetParent(VisualElement value)
		{
			m_Owner.m_PhysicalParent = value;
			m_Owner.m_LogicalParent = value;
			if (value != null)
			{
				m_Owner.ChangePanel(m_Owner.m_PhysicalParent.elementPanel);
				m_Owner.PropagateChangesToParents();
			}
			else
			{
				m_Owner.ChangePanel(null);
			}
		}

		public void Sort(Comparison<VisualElement> comp)
		{
			if (childCount > 0)
			{
				m_Owner.m_Children.Sort(comp);
				m_Owner.cssNode.Clear();
				for (int i = 0; i < m_Owner.m_Children.Count; i++)
				{
					m_Owner.cssNode.Insert(i, m_Owner.m_Children[i].cssNode);
				}
				m_Owner.Dirty(ChangeType.Layout);
			}
		}

		private void PutChildAtIndex(VisualElement child, int index)
		{
			if (index >= childCount)
			{
				m_Owner.m_Children.Add(child);
				m_Owner.cssNode.Insert(m_Owner.cssNode.Count, child.cssNode);
			}
			else
			{
				m_Owner.m_Children.Insert(index, child);
				m_Owner.cssNode.Insert(index, child.cssNode);
			}
		}

		private void RemoveChildAtIndex(int index)
		{
			m_Owner.m_Children.RemoveAt(index);
			m_Owner.cssNode.RemoveAt(index);
		}
	}

	private abstract class BaseVisualElementScheduledItem : ScheduledItem, IVisualElementScheduledItem, IVisualElementPanelActivatable
	{
		public bool isScheduled = false;

		private VisualElementPanelActivator m_Activator;

		public VisualElement element { get; private set; }

		public bool isActive => m_Activator.isActive;

		protected BaseVisualElementScheduledItem(VisualElement handler)
		{
			element = handler;
			m_Activator = new VisualElementPanelActivator(this);
		}

		public IVisualElementScheduledItem StartingIn(long delayMs)
		{
			base.delayMs = delayMs;
			return this;
		}

		public IVisualElementScheduledItem Until(Func<bool> stopCondition)
		{
			if (stopCondition == null)
			{
				stopCondition = ScheduledItem.ForeverCondition;
			}
			timerUpdateStopCondition = stopCondition;
			return this;
		}

		public IVisualElementScheduledItem ForDuration(long durationMs)
		{
			SetDuration(durationMs);
			return this;
		}

		public IVisualElementScheduledItem Every(long intervalMs)
		{
			base.intervalMs = intervalMs;
			if (timerUpdateStopCondition == ScheduledItem.OnceCondition)
			{
				timerUpdateStopCondition = ScheduledItem.ForeverCondition;
			}
			return this;
		}

		internal override void OnItemUnscheduled()
		{
			base.OnItemUnscheduled();
			isScheduled = false;
			if (!m_Activator.isDetaching)
			{
				m_Activator.SetActive(action: false);
			}
		}

		public void Resume()
		{
			isScheduled = true;
			m_Activator.SetActive(action: true);
		}

		public void Pause()
		{
			m_Activator.SetActive(action: false);
		}

		public void ExecuteLater(long delayMs)
		{
			if (!isScheduled)
			{
				Resume();
			}
			ResetStartTime();
			StartingIn(delayMs);
		}

		public void OnPanelActivate()
		{
			isScheduled = true;
			ResetStartTime();
			element.elementPanel.scheduler.Schedule(this);
		}

		public void OnPanelDeactivate()
		{
			if (isScheduled)
			{
				isScheduled = false;
				element.elementPanel.scheduler.Unschedule(this);
			}
		}

		public bool CanBeActivated()
		{
			return element != null && element.elementPanel != null && element.elementPanel.scheduler != null;
		}
	}

	private abstract class VisualElementScheduledItem<ActionType> : BaseVisualElementScheduledItem
	{
		public ActionType updateEvent;

		public VisualElementScheduledItem(VisualElement handler, ActionType upEvent)
			: base(handler)
		{
			updateEvent = upEvent;
		}

		public static bool Matches(ScheduledItem item, ActionType updateEvent)
		{
			if (item is VisualElementScheduledItem<ActionType> visualElementScheduledItem)
			{
				return EqualityComparer<ActionType>.Default.Equals(visualElementScheduledItem.updateEvent, updateEvent);
			}
			return false;
		}
	}

	private class TimerStateScheduledItem : VisualElementScheduledItem<Action<TimerState>>
	{
		public TimerStateScheduledItem(VisualElement handler, Action<TimerState> updateEvent)
			: base(handler, updateEvent)
		{
		}

		public override void PerformTimerUpdate(TimerState state)
		{
			if (isScheduled)
			{
				updateEvent(state);
			}
		}
	}

	private class SimpleScheduledItem : VisualElementScheduledItem<Action>
	{
		public SimpleScheduledItem(VisualElement handler, Action updateEvent)
			: base(handler, updateEvent)
		{
		}

		public override void PerformTimerUpdate(TimerState state)
		{
			if (isScheduled)
			{
				updateEvent();
			}
		}
	}

	private static uint s_NextId;

	private string m_Name;

	private HashSet<string> m_ClassList;

	private string m_TypeName;

	private string m_FullTypeName;

	private string m_PersistenceKey;

	private RenderData m_RenderData;

	private Vector3 m_Position = Vector3.zero;

	private Quaternion m_Rotation = Quaternion.identity;

	private Vector3 m_Scale = Vector3.one;

	private Rect m_Layout;

	internal PseudoStates triggerPseudoMask;

	internal PseudoStates dependencyPseudoMask;

	private PseudoStates m_PseudoStates;

	internal VisualElementStylesData m_SharedStyle = VisualElementStylesData.none;

	internal VisualElementStylesData m_Style = VisualElementStylesData.none;

	internal readonly uint controlid;

	private ChangeType changesNeeded;

	private bool m_Enabled;

	internal const Align DefaultAlignContent = Align.FlexStart;

	internal const Align DefaultAlignItems = Align.Stretch;

	private ClippingOptions m_ClippingOptions;

	private VisualElement m_PhysicalParent;

	private VisualElement m_LogicalParent;

	private static readonly VisualElement[] s_EmptyList = new VisualElement[0];

	private List<VisualElement> m_Children;

	private List<StyleSheet> m_StyleSheets;

	private List<string> m_StyleSheetPaths;

	Vector3 ITransform.position
	{
		get
		{
			return m_Position;
		}
		set
		{
			if (!(m_Position == value))
			{
				m_Position = value;
				Dirty(ChangeType.Transform);
			}
		}
	}

	Quaternion ITransform.rotation
	{
		get
		{
			return m_Rotation;
		}
		set
		{
			if (!(m_Rotation == value))
			{
				m_Rotation = value;
				Dirty(ChangeType.Transform);
			}
		}
	}

	Vector3 ITransform.scale
	{
		get
		{
			return m_Scale;
		}
		set
		{
			if (!(m_Scale == value))
			{
				m_Scale = value;
				Dirty(ChangeType.Transform);
			}
		}
	}

	Matrix4x4 ITransform.matrix => Matrix4x4.TRS(m_Position, m_Rotation, m_Scale);

	StyleValue<float> IStyle.width
	{
		get
		{
			return effectiveStyle.width;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.width, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.Width = value.value;
			}
		}
	}

	StyleValue<float> IStyle.height
	{
		get
		{
			return effectiveStyle.height;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.height, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.Height = value.value;
			}
		}
	}

	StyleValue<float> IStyle.maxWidth
	{
		get
		{
			return effectiveStyle.maxWidth;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.maxWidth, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.MaxWidth = value.value;
			}
		}
	}

	StyleValue<float> IStyle.maxHeight
	{
		get
		{
			return effectiveStyle.maxHeight;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.maxHeight, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.MaxHeight = value.value;
			}
		}
	}

	StyleValue<float> IStyle.minWidth
	{
		get
		{
			return effectiveStyle.minWidth;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.minWidth, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.MinWidth = value.value;
			}
		}
	}

	StyleValue<float> IStyle.minHeight
	{
		get
		{
			return effectiveStyle.minHeight;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.minHeight, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.MinHeight = value.value;
			}
		}
	}

	StyleValue<float> IStyle.flex
	{
		get
		{
			return effectiveStyle.flex;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.flex, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.Flex = value.value;
			}
		}
	}

	StyleValue<float> IStyle.flexBasis
	{
		get
		{
			return effectiveStyle.flexBasis;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.flexBasis, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.FlexBasis = value.value;
			}
		}
	}

	StyleValue<float> IStyle.flexGrow
	{
		get
		{
			return effectiveStyle.flexGrow;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.flexGrow, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.FlexGrow = value.value;
			}
		}
	}

	StyleValue<float> IStyle.flexShrink
	{
		get
		{
			return effectiveStyle.flexShrink;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.flexShrink, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.FlexShrink = value.value;
			}
		}
	}

	StyleValue<Overflow> IStyle.overflow
	{
		get
		{
			return new StyleValue<Overflow>((Overflow)effectiveStyle.overflow.value, effectiveStyle.overflow.specificity);
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.overflow, new StyleValue<int>((int)value.value, value.specificity)))
			{
				Dirty(ChangeType.Layout);
				cssNode.Overflow = (CSSOverflow)value.value;
			}
		}
	}

	StyleValue<float> IStyle.positionLeft
	{
		get
		{
			return effectiveStyle.positionLeft;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.positionLeft, value))
			{
				ChangeType changeType = changesNeeded;
				Dirty(ChangeType.Layout);
				if ((changeType & ChangeType.Repaint) == 0)
				{
					ClearDirty(ChangeType.Repaint);
				}
				cssNode.SetPosition(CSSEdge.Left, value.value);
			}
		}
	}

	StyleValue<float> IStyle.positionTop
	{
		get
		{
			return effectiveStyle.positionTop;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.positionTop, value))
			{
				ChangeType changeType = changesNeeded;
				Dirty(ChangeType.Layout);
				if ((changeType & ChangeType.Repaint) == 0)
				{
					ClearDirty(ChangeType.Repaint);
				}
				cssNode.SetPosition(CSSEdge.Top, value.value);
			}
		}
	}

	StyleValue<float> IStyle.positionRight
	{
		get
		{
			return effectiveStyle.positionRight;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.positionRight, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.SetPosition(CSSEdge.Right, value.value);
			}
		}
	}

	StyleValue<float> IStyle.positionBottom
	{
		get
		{
			return effectiveStyle.positionBottom;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.positionBottom, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.SetPosition(CSSEdge.Bottom, value.value);
			}
		}
	}

	StyleValue<float> IStyle.marginLeft
	{
		get
		{
			return effectiveStyle.marginLeft;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.marginLeft, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.SetMargin(CSSEdge.Left, value.value);
			}
		}
	}

	StyleValue<float> IStyle.marginTop
	{
		get
		{
			return effectiveStyle.marginTop;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.marginTop, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.SetMargin(CSSEdge.Top, value.value);
			}
		}
	}

	StyleValue<float> IStyle.marginRight
	{
		get
		{
			return effectiveStyle.marginRight;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.marginRight, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.SetMargin(CSSEdge.Right, value.value);
			}
		}
	}

	StyleValue<float> IStyle.marginBottom
	{
		get
		{
			return effectiveStyle.marginBottom;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.marginBottom, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.SetMargin(CSSEdge.Bottom, value.value);
			}
		}
	}

	StyleValue<float> IStyle.borderLeft
	{
		get
		{
			return effectiveStyle.borderLeft;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.borderLeft, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.SetBorder(CSSEdge.Left, value.value);
			}
		}
	}

	StyleValue<float> IStyle.borderTop
	{
		get
		{
			return effectiveStyle.borderTop;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.borderTop, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.SetBorder(CSSEdge.Top, value.value);
			}
		}
	}

	StyleValue<float> IStyle.borderRight
	{
		get
		{
			return effectiveStyle.borderRight;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.borderRight, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.SetBorder(CSSEdge.Right, value.value);
			}
		}
	}

	StyleValue<float> IStyle.borderBottom
	{
		get
		{
			return effectiveStyle.borderBottom;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.borderBottom, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.SetBorder(CSSEdge.Bottom, value.value);
			}
		}
	}

	StyleValue<float> IStyle.borderLeftWidth
	{
		get
		{
			return effectiveStyle.borderLeftWidth;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.borderLeftWidth, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.SetBorder(CSSEdge.Left, value.value);
			}
		}
	}

	StyleValue<float> IStyle.borderTopWidth
	{
		get
		{
			return effectiveStyle.borderTopWidth;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.borderTopWidth, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.SetBorder(CSSEdge.Top, value.value);
			}
		}
	}

	StyleValue<float> IStyle.borderRightWidth
	{
		get
		{
			return effectiveStyle.borderRightWidth;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.borderRightWidth, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.SetBorder(CSSEdge.Right, value.value);
			}
		}
	}

	StyleValue<float> IStyle.borderBottomWidth
	{
		get
		{
			return effectiveStyle.borderBottomWidth;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.borderBottomWidth, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.SetBorder(CSSEdge.Bottom, value.value);
			}
		}
	}

	StyleValue<float> IStyle.borderRadius
	{
		get
		{
			return style.borderTopLeftRadius;
		}
		set
		{
			style.borderTopLeftRadius = value;
			style.borderTopRightRadius = value;
			style.borderBottomLeftRadius = value;
			style.borderBottomRightRadius = value;
		}
	}

	StyleValue<float> IStyle.borderTopLeftRadius
	{
		get
		{
			return effectiveStyle.borderTopLeftRadius;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.borderTopLeftRadius, value))
			{
				Dirty(ChangeType.Repaint);
			}
		}
	}

	StyleValue<float> IStyle.borderTopRightRadius
	{
		get
		{
			return effectiveStyle.borderTopRightRadius;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.borderTopRightRadius, value))
			{
				Dirty(ChangeType.Repaint);
			}
		}
	}

	StyleValue<float> IStyle.borderBottomRightRadius
	{
		get
		{
			return effectiveStyle.borderBottomRightRadius;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.borderBottomRightRadius, value))
			{
				Dirty(ChangeType.Repaint);
			}
		}
	}

	StyleValue<float> IStyle.borderBottomLeftRadius
	{
		get
		{
			return effectiveStyle.borderBottomLeftRadius;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.borderBottomLeftRadius, value))
			{
				Dirty(ChangeType.Repaint);
			}
		}
	}

	StyleValue<float> IStyle.paddingLeft
	{
		get
		{
			return effectiveStyle.paddingLeft;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.paddingLeft, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.SetPadding(CSSEdge.Left, value.value);
			}
		}
	}

	StyleValue<float> IStyle.paddingTop
	{
		get
		{
			return effectiveStyle.paddingTop;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.paddingTop, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.SetPadding(CSSEdge.Top, value.value);
			}
		}
	}

	StyleValue<float> IStyle.paddingRight
	{
		get
		{
			return effectiveStyle.paddingRight;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.paddingRight, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.SetPadding(CSSEdge.Right, value.value);
			}
		}
	}

	StyleValue<float> IStyle.paddingBottom
	{
		get
		{
			return effectiveStyle.paddingBottom;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.paddingBottom, value))
			{
				Dirty(ChangeType.Layout);
				cssNode.SetPadding(CSSEdge.Bottom, value.value);
			}
		}
	}

	StyleValue<PositionType> IStyle.positionType
	{
		get
		{
			return new StyleValue<PositionType>((PositionType)effectiveStyle.positionType.value, effectiveStyle.positionType.specificity);
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.positionType, new StyleValue<int>((int)value.value, value.specificity)))
			{
				Dirty(ChangeType.Layout);
				switch (value.value)
				{
				case PositionType.Absolute:
				case PositionType.Manual:
					cssNode.PositionType = CSSPositionType.Absolute;
					break;
				case PositionType.Relative:
					cssNode.PositionType = CSSPositionType.Relative;
					break;
				}
			}
		}
	}

	StyleValue<Align> IStyle.alignSelf
	{
		get
		{
			return new StyleValue<Align>((Align)effectiveStyle.alignSelf.value, effectiveStyle.alignSelf.specificity);
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.alignSelf, new StyleValue<int>((int)value.value, value.specificity)))
			{
				Dirty(ChangeType.Layout);
				cssNode.AlignSelf = (CSSAlign)value.value;
			}
		}
	}

	StyleValue<TextAnchor> IStyle.textAlignment
	{
		get
		{
			return new StyleValue<TextAnchor>((TextAnchor)effectiveStyle.textAlignment.value, effectiveStyle.textAlignment.specificity);
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.textAlignment, new StyleValue<int>((int)value.value, value.specificity)))
			{
				Dirty(ChangeType.Repaint);
			}
		}
	}

	StyleValue<FontStyle> IStyle.fontStyle
	{
		get
		{
			return new StyleValue<FontStyle>((FontStyle)effectiveStyle.fontStyle.value, effectiveStyle.fontStyle.specificity);
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.fontStyle, new StyleValue<int>((int)value.value, value.specificity)))
			{
				Dirty(ChangeType.Layout);
			}
		}
	}

	StyleValue<TextClipping> IStyle.textClipping
	{
		get
		{
			return new StyleValue<TextClipping>((TextClipping)effectiveStyle.textClipping.value, effectiveStyle.textClipping.specificity);
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.textClipping, new StyleValue<int>((int)value.value, value.specificity)))
			{
				Dirty(ChangeType.Repaint);
			}
		}
	}

	StyleValue<Font> IStyle.font
	{
		get
		{
			return effectiveStyle.font;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.font, value))
			{
				Dirty(ChangeType.Layout);
			}
		}
	}

	StyleValue<int> IStyle.fontSize
	{
		get
		{
			return effectiveStyle.fontSize;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.fontSize, value))
			{
				Dirty(ChangeType.Layout);
			}
		}
	}

	StyleValue<bool> IStyle.wordWrap
	{
		get
		{
			return effectiveStyle.wordWrap;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.wordWrap, value))
			{
				Dirty(ChangeType.Layout);
			}
		}
	}

	StyleValue<Color> IStyle.textColor
	{
		get
		{
			return effectiveStyle.textColor;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.textColor, value))
			{
				Dirty(ChangeType.Repaint);
			}
		}
	}

	StyleValue<FlexDirection> IStyle.flexDirection
	{
		get
		{
			return new StyleValue<FlexDirection>((FlexDirection)effectiveStyle.flexDirection.value, effectiveStyle.flexDirection.specificity);
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.flexDirection, new StyleValue<int>((int)value.value, value.specificity)))
			{
				Dirty(ChangeType.Repaint);
				cssNode.FlexDirection = (CSSFlexDirection)value.value;
			}
		}
	}

	StyleValue<Color> IStyle.backgroundColor
	{
		get
		{
			return effectiveStyle.backgroundColor;
		}
		set
		{
			if (value.specificity == 0 && value == default(Color))
			{
				inlineStyle.backgroundColor = sharedStyle.backgroundColor;
				Dirty(ChangeType.Repaint);
			}
			else if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.backgroundColor, value))
			{
				Dirty(ChangeType.Repaint);
			}
		}
	}

	StyleValue<Color> IStyle.borderColor
	{
		get
		{
			return effectiveStyle.borderColor;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.borderColor, value))
			{
				Dirty(ChangeType.Repaint);
			}
		}
	}

	StyleValue<Texture2D> IStyle.backgroundImage
	{
		get
		{
			return effectiveStyle.backgroundImage;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.backgroundImage, value))
			{
				Dirty(ChangeType.Repaint);
			}
		}
	}

	StyleValue<ScaleMode> IStyle.backgroundSize
	{
		get
		{
			return new StyleValue<ScaleMode>((ScaleMode)effectiveStyle.backgroundSize.value, effectiveStyle.backgroundSize.specificity);
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.backgroundSize, new StyleValue<int>((int)value.value, value.specificity)))
			{
				Dirty(ChangeType.Repaint);
			}
		}
	}

	StyleValue<Align> IStyle.alignItems
	{
		get
		{
			return new StyleValue<Align>((Align)effectiveStyle.alignItems.value, effectiveStyle.alignItems.specificity);
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.alignItems, new StyleValue<int>((int)value.value, value.specificity)))
			{
				Dirty(ChangeType.Layout);
				cssNode.AlignItems = (CSSAlign)value.value;
			}
		}
	}

	StyleValue<Align> IStyle.alignContent
	{
		get
		{
			return new StyleValue<Align>((Align)effectiveStyle.alignContent.value, effectiveStyle.alignContent.specificity);
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.alignContent, new StyleValue<int>((int)value.value, value.specificity)))
			{
				Dirty(ChangeType.Layout);
				cssNode.AlignContent = (CSSAlign)value.value;
			}
		}
	}

	StyleValue<Justify> IStyle.justifyContent
	{
		get
		{
			return new StyleValue<Justify>((Justify)effectiveStyle.justifyContent.value, effectiveStyle.justifyContent.specificity);
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.justifyContent, new StyleValue<int>((int)value.value, value.specificity)))
			{
				Dirty(ChangeType.Layout);
				cssNode.JustifyContent = (CSSJustify)value.value;
			}
		}
	}

	StyleValue<Wrap> IStyle.flexWrap
	{
		get
		{
			return new StyleValue<Wrap>((Wrap)effectiveStyle.flexWrap.value, effectiveStyle.flexWrap.specificity);
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.flexWrap, new StyleValue<int>((int)value.value, value.specificity)))
			{
				Dirty(ChangeType.Layout);
				cssNode.Wrap = (CSSWrap)value.value;
			}
		}
	}

	StyleValue<int> IStyle.sliceLeft
	{
		get
		{
			return effectiveStyle.sliceLeft;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.sliceLeft, value))
			{
				Dirty(ChangeType.Repaint);
			}
		}
	}

	StyleValue<int> IStyle.sliceTop
	{
		get
		{
			return effectiveStyle.sliceTop;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.sliceTop, value))
			{
				Dirty(ChangeType.Repaint);
			}
		}
	}

	StyleValue<int> IStyle.sliceRight
	{
		get
		{
			return effectiveStyle.sliceRight;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.sliceRight, value))
			{
				Dirty(ChangeType.Repaint);
			}
		}
	}

	StyleValue<int> IStyle.sliceBottom
	{
		get
		{
			return effectiveStyle.sliceBottom;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.sliceBottom, value))
			{
				Dirty(ChangeType.Repaint);
			}
		}
	}

	StyleValue<float> IStyle.opacity
	{
		get
		{
			return effectiveStyle.opacity;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref inlineStyle.opacity, value))
			{
				Dirty(ChangeType.Repaint);
			}
		}
	}

	StyleValue<CursorStyle> IStyle.cursor
	{
		get
		{
			return effectiveStyle.cursor;
		}
		set
		{
			StyleValueUtils.ApplyAndCompare(ref inlineStyle.cursor, value);
		}
	}

	/// <summary>
	///   <para>Used for view data persistence (ie. tree expanded states, scroll position, zoom level).</para>
	/// </summary>
	public string persistenceKey
	{
		get
		{
			return m_PersistenceKey;
		}
		set
		{
			if (m_PersistenceKey != value)
			{
				m_PersistenceKey = value;
				if (!string.IsNullOrEmpty(value))
				{
					Dirty(ChangeType.PersistentData);
				}
			}
		}
	}

	internal bool enablePersistence { get; private set; }

	/// <summary>
	///   <para>This property can be used to associate application-specific user data with this VisualElement.</para>
	/// </summary>
	public object userData { get; set; }

	public override bool canGrabFocus => enabledInHierarchy && base.canGrabFocus;

	public override FocusController focusController => (panel != null) ? panel.focusController : null;

	internal RenderData renderData => m_RenderData ?? (m_RenderData = new RenderData());

	public ITransform transform => this;

	public Rect layout
	{
		get
		{
			Rect result = m_Layout;
			if (cssNode != null && style.positionType.value != PositionType.Manual)
			{
				result.x = cssNode.LayoutX;
				result.y = cssNode.LayoutY;
				result.width = cssNode.LayoutWidth;
				result.height = cssNode.LayoutHeight;
			}
			return result;
		}
		set
		{
			if (cssNode == null)
			{
				cssNode = new CSSNode();
			}
			if (style.positionType.value != PositionType.Manual || !(m_Layout == value))
			{
				m_Layout = value;
				((IStyle)this).positionType = PositionType.Manual;
				((IStyle)this).marginLeft = 0f;
				((IStyle)this).marginRight = 0f;
				((IStyle)this).marginBottom = 0f;
				((IStyle)this).marginTop = 0f;
				((IStyle)this).positionLeft = value.x;
				((IStyle)this).positionTop = value.y;
				((IStyle)this).positionRight = float.NaN;
				((IStyle)this).positionBottom = float.NaN;
				((IStyle)this).width = value.width;
				((IStyle)this).height = value.height;
				Dirty(ChangeType.Transform);
			}
		}
	}

	public Rect contentRect
	{
		get
		{
			Spacing spacing = new Spacing(m_Style.paddingLeft, m_Style.paddingTop, m_Style.paddingRight, m_Style.paddingBottom);
			return paddingRect - spacing;
		}
	}

	protected Rect paddingRect
	{
		get
		{
			Spacing spacing = new Spacing(style.borderLeftWidth, style.borderTopWidth, style.borderRightWidth, style.borderBottomWidth);
			return rect - spacing;
		}
	}

	public Rect worldBound
	{
		get
		{
			Matrix4x4 matrix4x = worldTransform;
			Vector3 vector = GUIUtility.Internal_MultiplyPoint(new Vector3(rect.min.x, rect.min.y, 1f), matrix4x);
			Vector3 vector2 = GUIUtility.Internal_MultiplyPoint(new Vector3(rect.max.x, rect.max.y, 1f), matrix4x);
			return Rect.MinMaxRect(Math.Min(vector.x, vector2.x), Math.Min(vector.y, vector2.y), Math.Max(vector.x, vector2.x), Math.Max(vector.y, vector2.y));
		}
	}

	public Rect localBound
	{
		get
		{
			Matrix4x4 matrix = transform.matrix;
			Vector3 vector = GUIUtility.Internal_MultiplyPoint(layout.min, matrix);
			Vector3 vector2 = GUIUtility.Internal_MultiplyPoint(layout.max, matrix);
			return Rect.MinMaxRect(Math.Min(vector.x, vector2.x), Math.Min(vector.y, vector2.y), Math.Max(vector.x, vector2.x), Math.Max(vector.y, vector2.y));
		}
	}

	internal Rect rect => new Rect(0f, 0f, layout.width, layout.height);

	internal Rect alignedRect => GUIUtility.Internal_AlignRectToDevice(rect, worldTransform);

	public Matrix4x4 worldTransform
	{
		get
		{
			if (IsDirty(ChangeType.Transform))
			{
				Matrix4x4 matrix4x = Matrix4x4.Translate(new Vector3(layout.x, layout.y, 0f));
				if (shadow.parent != null)
				{
					renderData.worldTransForm = shadow.parent.worldTransform * matrix4x * transform.matrix;
				}
				else
				{
					renderData.worldTransForm = matrix4x * transform.matrix;
				}
				ClearDirty(ChangeType.Transform);
			}
			return renderData.worldTransForm;
		}
	}

	internal PseudoStates pseudoStates
	{
		get
		{
			return m_PseudoStates;
		}
		set
		{
			if (m_PseudoStates != value)
			{
				m_PseudoStates = value;
				if ((triggerPseudoMask & m_PseudoStates) != 0 || (dependencyPseudoMask & ~m_PseudoStates) != 0)
				{
					Dirty(ChangeType.Styles);
				}
			}
		}
	}

	public PickingMode pickingMode { get; set; }

	public string name
	{
		get
		{
			return m_Name;
		}
		set
		{
			if (!(m_Name == value))
			{
				m_Name = value;
				Dirty(ChangeType.Styles);
			}
		}
	}

	internal string fullTypeName
	{
		get
		{
			if (string.IsNullOrEmpty(m_FullTypeName))
			{
				m_FullTypeName = GetType().FullName;
			}
			return m_FullTypeName;
		}
	}

	internal string typeName
	{
		get
		{
			if (string.IsNullOrEmpty(m_TypeName))
			{
				m_TypeName = GetType().Name;
			}
			return m_TypeName;
		}
	}

	internal CSSNode cssNode { get; private set; }

	internal VisualElementStylesData sharedStyle => m_SharedStyle;

	internal VisualElementStylesData effectiveStyle => m_Style;

	internal bool hasInlineStyle => m_Style != m_SharedStyle;

	private VisualElementStylesData inlineStyle
	{
		get
		{
			if (!hasInlineStyle)
			{
				VisualElementStylesData visualElementStylesData = new VisualElementStylesData(isShared: false);
				visualElementStylesData.Apply(m_SharedStyle, StylePropertyApplyMode.Copy);
				m_Style = visualElementStylesData;
			}
			return m_Style;
		}
	}

	internal float opacity
	{
		get
		{
			return style.opacity.value;
		}
		set
		{
			style.opacity = value;
		}
	}

	[Obsolete("enabled is deprecated. Use SetEnabled as setter, and enabledSelf/enabledInHierarchy as getters.", true)]
	public virtual bool enabled
	{
		get
		{
			return enabledInHierarchy;
		}
		set
		{
			SetEnabled(value);
		}
	}

	/// <summary>
	///   <para>Returns true if the VisualElement is enabled in its own hierarchy.</para>
	/// </summary>
	public bool enabledInHierarchy => (pseudoStates & PseudoStates.Disabled) != PseudoStates.Disabled;

	/// <summary>
	///   <para>Returns true if the VisualElement is enabled locally.</para>
	/// </summary>
	public bool enabledSelf => m_Enabled;

	public bool visible
	{
		get
		{
			return (pseudoStates & PseudoStates.Invisible) != PseudoStates.Invisible;
		}
		set
		{
			if (value)
			{
				pseudoStates &= (PseudoStates)2147483647;
			}
			else
			{
				pseudoStates |= PseudoStates.Invisible;
			}
		}
	}

	/// <summary>
	///   <para>Access to this element data watch interface.</para>
	/// </summary>
	public IUIElementDataWatch dataWatch => this;

	/// <summary>
	///   <para> Access to this element physical hierarchy
	///           </para>
	/// </summary>
	public Hierarchy shadow { get; private set; }

	/// <summary>
	///   <para>Should this element clip painting to its boundaries.</para>
	/// </summary>
	public ClippingOptions clippingOptions
	{
		get
		{
			return m_ClippingOptions;
		}
		set
		{
			if (m_ClippingOptions != value)
			{
				m_ClippingOptions = value;
				Dirty(ChangeType.Repaint);
			}
		}
	}

	public VisualElement parent => m_LogicalParent;

	internal BaseVisualElementPanel elementPanel { get; private set; }

	public IPanel panel => elementPanel;

	/// <summary>
	///   <para> child elements are added to this element, usually this
	///           </para>
	/// </summary>
	public virtual VisualElement contentContainer => this;

	public VisualElement this[int key] => ElementAt(key);

	/// <summary>
	///   <para> Number of child elements in this object's contentContainer
	///           </para>
	/// </summary>
	public int childCount
	{
		get
		{
			if (contentContainer == this)
			{
				return shadow.childCount;
			}
			return contentContainer.childCount;
		}
	}

	/// <summary>
	///   <para>Retrieves this VisualElement's IVisualElementScheduler</para>
	/// </summary>
	public IVisualElementScheduler schedule => this;

	/// <summary>
	///   <para>Reference to the style object of this element.</para>
	/// </summary>
	public IStyle style => this;

	internal IList<StyleSheet> styleSheets
	{
		get
		{
			if (m_StyleSheets == null && m_StyleSheetPaths != null)
			{
				LoadStyleSheetsFromPaths();
			}
			return m_StyleSheets;
		}
	}

	internal event OnStylesResolved onStylesResolved;

	public VisualElement()
	{
		controlid = ++s_NextId;
		shadow = new Hierarchy(this);
		m_ClassList = new HashSet<string>();
		m_FullTypeName = string.Empty;
		m_TypeName = string.Empty;
		SetEnabled(value: true);
		visible = true;
		base.focusIndex = -1;
		name = string.Empty;
		cssNode = new CSSNode();
		cssNode.SetMeasureFunction(Measure);
		changesNeeded = ChangeType.All;
		clippingOptions = ClippingOptions.ClipContents;
	}

	/// <summary>
	///   <para>Callback when the styles of an object have changed.</para>
	/// </summary>
	/// <param name="style"></param>
	protected virtual void OnStyleResolved(ICustomStyle style)
	{
		FinalizeLayout();
	}

	protected internal override void ExecuteDefaultAction(EventBase evt)
	{
		base.ExecuteDefaultAction(evt);
		if (evt.GetEventTypeId() == EventBase<MouseOverEvent>.TypeId() || evt.GetEventTypeId() == EventBase<MouseOutEvent>.TypeId())
		{
			UpdateCursorStyle(evt.GetEventTypeId());
		}
		else if (evt.GetEventTypeId() == EventBase<MouseEnterEvent>.TypeId())
		{
			pseudoStates |= PseudoStates.Hover;
		}
		else if (evt.GetEventTypeId() == EventBase<MouseLeaveEvent>.TypeId())
		{
			pseudoStates &= ~PseudoStates.Hover;
		}
		else if (evt.GetEventTypeId() == EventBase<BlurEvent>.TypeId())
		{
			pseudoStates &= ~PseudoStates.Focus;
		}
		else if (evt.GetEventTypeId() == EventBase<FocusEvent>.TypeId())
		{
			pseudoStates |= PseudoStates.Focus;
		}
	}

	public sealed override void Focus()
	{
		if (!canGrabFocus && shadow.parent != null)
		{
			shadow.parent.Focus();
		}
		else
		{
			base.Focus();
		}
	}

	internal virtual void ChangePanel(BaseVisualElementPanel p)
	{
		if (panel == p)
		{
			return;
		}
		if (panel != null)
		{
			using DetachFromPanelEvent detachFromPanelEvent = EventBase<DetachFromPanelEvent>.GetPooled();
			detachFromPanelEvent.target = this;
			UIElementsUtility.eventDispatcher.DispatchEvent(detachFromPanelEvent, panel);
		}
		elementPanel = p;
		if (panel != null)
		{
			using AttachToPanelEvent attachToPanelEvent = EventBase<AttachToPanelEvent>.GetPooled();
			attachToPanelEvent.target = this;
			UIElementsUtility.eventDispatcher.DispatchEvent(attachToPanelEvent, panel);
		}
		Dirty(ChangeType.Styles);
		if (m_Children != null && m_Children.Count > 0)
		{
			for (int i = 0; i < m_Children.Count; i++)
			{
				VisualElement visualElement = m_Children[i];
				visualElement.ChangePanel(p);
			}
		}
	}

	private void PropagateToChildren(ChangeType type)
	{
		if ((type & changesNeeded) == type)
		{
			return;
		}
		changesNeeded |= type;
		type &= ChangeType.Styles | ChangeType.Transform;
		if (type == (ChangeType)0 || m_Children == null)
		{
			return;
		}
		foreach (VisualElement child in m_Children)
		{
			child.PropagateToChildren(type);
		}
	}

	private void PropagateChangesToParents()
	{
		ChangeType changeType = (ChangeType)0;
		if (changesNeeded != 0)
		{
			changeType |= ChangeType.Repaint;
			if ((changesNeeded & ChangeType.Styles) > (ChangeType)0)
			{
				changeType |= ChangeType.StylesPath;
			}
			if ((changesNeeded & (ChangeType.PersistentData | ChangeType.PersistentDataPath)) > (ChangeType)0)
			{
				changeType |= ChangeType.PersistentDataPath;
			}
		}
		VisualElement visualElement = shadow.parent;
		while (visualElement != null && (visualElement.changesNeeded & changeType) != changeType)
		{
			visualElement.changesNeeded |= changeType;
			visualElement = visualElement.shadow.parent;
		}
	}

	public void Dirty(ChangeType type)
	{
		if ((type & changesNeeded) == type)
		{
			return;
		}
		if ((type & ChangeType.Layout) > (ChangeType)0)
		{
			if (cssNode != null && cssNode.IsMeasureDefined)
			{
				cssNode.MarkDirty();
			}
			type |= ChangeType.Repaint;
		}
		PropagateToChildren(type);
		PropagateChangesToParents();
	}

	internal bool AnyDirty()
	{
		return changesNeeded != (ChangeType)0;
	}

	public bool IsDirty(ChangeType type)
	{
		return (changesNeeded & type) == type;
	}

	/// <summary>
	///   <para>Checks if any of the ChangeTypes have been marked dirty.</para>
	/// </summary>
	/// <param name="type">The ChangeType(s) to check.</param>
	/// <returns>
	///   <para>True if at least one of the checked ChangeTypes have been marked dirty.</para>
	/// </returns>
	public bool AnyDirty(ChangeType type)
	{
		return (changesNeeded & type) > (ChangeType)0;
	}

	public void ClearDirty(ChangeType type)
	{
		changesNeeded &= ~type;
	}

	protected internal bool SetEnabledFromHierarchy(bool state)
	{
		if (state == ((pseudoStates & PseudoStates.Disabled) != PseudoStates.Disabled))
		{
			return false;
		}
		if (state && m_Enabled && (parent == null || parent.enabledInHierarchy))
		{
			pseudoStates &= ~PseudoStates.Disabled;
		}
		else
		{
			pseudoStates |= PseudoStates.Disabled;
		}
		return true;
	}

	/// <summary>
	///   <para>Changes the VisualElement enabled state. A disabled VisualElement does not receive most events.</para>
	/// </summary>
	/// <param name="value">New enabled state</param>
	public void SetEnabled(bool value)
	{
		if (m_Enabled != value)
		{
			m_Enabled = value;
			PropagateEnabledToChildren(value);
		}
	}

	private void PropagateEnabledToChildren(bool value)
	{
		if (SetEnabledFromHierarchy(value))
		{
			for (int i = 0; i < shadow.childCount; i++)
			{
				shadow[i].PropagateEnabledToChildren(value);
			}
		}
	}

	public virtual void DoRepaint()
	{
		IStylePainter stylePainter = elementPanel.stylePainter;
		stylePainter.DrawBackground(this);
		stylePainter.DrawBorder(this);
	}

	internal virtual void DoRepaint(IStylePainter painter)
	{
		if ((pseudoStates & PseudoStates.Invisible) != PseudoStates.Invisible)
		{
			DoRepaint();
		}
	}

	private void GetFullHierarchicalPersistenceKey(StringBuilder key)
	{
		if (parent != null)
		{
			parent.GetFullHierarchicalPersistenceKey(key);
		}
		if (!string.IsNullOrEmpty(persistenceKey))
		{
			key.Append("__");
			key.Append(persistenceKey);
		}
	}

	/// <summary>
	///   <para>Combine this VisualElement's VisualElement.persistenceKey with those of its parents to create a more unique key for use with VisualElement.GetOrCreatePersistentData.</para>
	/// </summary>
	/// <returns>
	///   <para>Full hierarchical persistence key.</para>
	/// </returns>
	public string GetFullHierarchicalPersistenceKey()
	{
		StringBuilder stringBuilder = new StringBuilder();
		GetFullHierarchicalPersistenceKey(stringBuilder);
		return stringBuilder.ToString();
	}

	public T GetOrCreatePersistentData<T>(object existing, string key) where T : class, new()
	{
		Debug.Assert(elementPanel != null, "VisualElement.elementPanel is null! Cannot load persistent data.");
		ISerializableJsonDictionary serializableJsonDictionary = ((elementPanel != null && elementPanel.getViewDataDictionary != null) ? elementPanel.getViewDataDictionary() : null);
		if (serializableJsonDictionary == null || string.IsNullOrEmpty(persistenceKey) || !enablePersistence)
		{
			if (existing != null)
			{
				return existing as T;
			}
			return new T();
		}
		string key2 = key + "__" + typeof(T).ToString();
		if (!serializableJsonDictionary.ContainsKey(key2))
		{
			serializableJsonDictionary.Set(key2, new T());
		}
		return serializableJsonDictionary.Get<T>(key2);
	}

	public T GetOrCreatePersistentData<T>(ScriptableObject existing, string key) where T : ScriptableObject
	{
		Debug.Assert(elementPanel != null, "VisualElement.elementPanel is null! Cannot load persistent data.");
		ISerializableJsonDictionary serializableJsonDictionary = ((elementPanel != null && elementPanel.getViewDataDictionary != null) ? elementPanel.getViewDataDictionary() : null);
		if (serializableJsonDictionary == null || string.IsNullOrEmpty(persistenceKey) || !enablePersistence)
		{
			if (existing != null)
			{
				return existing as T;
			}
			return ScriptableObject.CreateInstance<T>();
		}
		string key2 = key + "__" + typeof(T).ToString();
		if (!serializableJsonDictionary.ContainsKey(key2))
		{
			serializableJsonDictionary.Set(key2, ScriptableObject.CreateInstance<T>());
		}
		return serializableJsonDictionary.GetScriptable<T>(key2);
	}

	/// <summary>
	///   <para>Overwrite object from the persistent data store.</para>
	/// </summary>
	/// <param name="key">The key for the current VisualElement to be used with the persistence store on the EditorWindow.</param>
	/// <param name="obj">Object to overwrite.</param>
	public void OverwriteFromPersistedData(object obj, string key)
	{
		Debug.Assert(elementPanel != null, "VisualElement.elementPanel is null! Cannot load persistent data.");
		ISerializableJsonDictionary serializableJsonDictionary = ((elementPanel != null && elementPanel.getViewDataDictionary != null) ? elementPanel.getViewDataDictionary() : null);
		if (serializableJsonDictionary != null && !string.IsNullOrEmpty(persistenceKey) && enablePersistence)
		{
			string key2 = key + "__" + obj.GetType();
			if (!serializableJsonDictionary.ContainsKey(key2))
			{
				serializableJsonDictionary.Set(key2, obj);
			}
			else
			{
				serializableJsonDictionary.Overwrite(obj, key2);
			}
		}
	}

	/// <summary>
	///   <para>Write persistence data to file.</para>
	/// </summary>
	public void SavePersistentData()
	{
		if (elementPanel != null && elementPanel.savePersistentViewData != null && !string.IsNullOrEmpty(persistenceKey))
		{
			elementPanel.savePersistentViewData();
		}
	}

	internal bool IsPersitenceSupportedOnChildren()
	{
		if (GetType() == typeof(VisualElement))
		{
			return true;
		}
		if (string.IsNullOrEmpty(persistenceKey))
		{
			return false;
		}
		return true;
	}

	internal void OnPersistentDataReady(bool enablePersistence)
	{
		this.enablePersistence = enablePersistence;
		OnPersistentDataReady();
	}

	/// <summary>
	///   <para>Called when the persistent data is accessible and/or when the data or persistence key have changed (VisualElement is properly parented).</para>
	/// </summary>
	public virtual void OnPersistentDataReady()
	{
	}

	public virtual bool ContainsPoint(Vector2 localPoint)
	{
		return rect.Contains(localPoint);
	}

	public virtual bool Overlaps(Rect rectangle)
	{
		return rect.Overlaps(rectangle, allowInverse: true);
	}

	protected internal virtual Vector2 DoMeasure(float width, MeasureMode widthMode, float height, MeasureMode heightMode)
	{
		return new Vector2(float.NaN, float.NaN);
	}

	internal long Measure(CSSNode node, float width, CSSMeasureMode widthMode, float height, CSSMeasureMode heightMode)
	{
		Debug.Assert(node == cssNode, "CSSNode instance mismatch");
		Vector2 vector = DoMeasure(width, (MeasureMode)widthMode, height, (MeasureMode)heightMode);
		return MeasureOutput.Make(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
	}

	public void SetSize(Vector2 size)
	{
		Rect rect = layout;
		rect.width = size.x;
		rect.height = size.y;
		layout = rect;
	}

	private void FinalizeLayout()
	{
		cssNode.Flex = style.flex.GetSpecifiedValueOrDefault(float.NaN);
		cssNode.FlexBasis = style.flexBasis.GetSpecifiedValueOrDefault(float.NaN);
		cssNode.FlexGrow = style.flexGrow.GetSpecifiedValueOrDefault(float.NaN);
		cssNode.FlexShrink = style.flexShrink.GetSpecifiedValueOrDefault(float.NaN);
		cssNode.SetPosition(CSSEdge.Left, style.positionLeft.GetSpecifiedValueOrDefault(float.NaN));
		cssNode.SetPosition(CSSEdge.Top, style.positionTop.GetSpecifiedValueOrDefault(float.NaN));
		cssNode.SetPosition(CSSEdge.Right, style.positionRight.GetSpecifiedValueOrDefault(float.NaN));
		cssNode.SetPosition(CSSEdge.Bottom, style.positionBottom.GetSpecifiedValueOrDefault(float.NaN));
		cssNode.SetMargin(CSSEdge.Left, style.marginLeft.GetSpecifiedValueOrDefault(float.NaN));
		cssNode.SetMargin(CSSEdge.Top, style.marginTop.GetSpecifiedValueOrDefault(float.NaN));
		cssNode.SetMargin(CSSEdge.Right, style.marginRight.GetSpecifiedValueOrDefault(float.NaN));
		cssNode.SetMargin(CSSEdge.Bottom, style.marginBottom.GetSpecifiedValueOrDefault(float.NaN));
		cssNode.SetPadding(CSSEdge.Left, style.paddingLeft.GetSpecifiedValueOrDefault(float.NaN));
		cssNode.SetPadding(CSSEdge.Top, style.paddingTop.GetSpecifiedValueOrDefault(float.NaN));
		cssNode.SetPadding(CSSEdge.Right, style.paddingRight.GetSpecifiedValueOrDefault(float.NaN));
		cssNode.SetPadding(CSSEdge.Bottom, style.paddingBottom.GetSpecifiedValueOrDefault(float.NaN));
		cssNode.SetBorder(CSSEdge.Left, style.borderLeft.GetSpecifiedValueOrDefault(style.borderLeftWidth.GetSpecifiedValueOrDefault(float.NaN)));
		cssNode.SetBorder(CSSEdge.Top, style.borderTop.GetSpecifiedValueOrDefault(style.borderTopWidth.GetSpecifiedValueOrDefault(float.NaN)));
		cssNode.SetBorder(CSSEdge.Right, style.borderRight.GetSpecifiedValueOrDefault(style.borderRightWidth.GetSpecifiedValueOrDefault(float.NaN)));
		cssNode.SetBorder(CSSEdge.Bottom, style.borderBottom.GetSpecifiedValueOrDefault(style.borderBottomWidth.GetSpecifiedValueOrDefault(float.NaN)));
		cssNode.Width = style.width.GetSpecifiedValueOrDefault(float.NaN);
		cssNode.Height = style.height.GetSpecifiedValueOrDefault(float.NaN);
		switch (style.positionType)
		{
		case 1L:
		case 2L:
			cssNode.PositionType = CSSPositionType.Absolute;
			break;
		case 0L:
			cssNode.PositionType = CSSPositionType.Relative;
			break;
		}
		cssNode.Overflow = (CSSOverflow)style.overflow.value;
		cssNode.AlignSelf = (CSSAlign)style.alignSelf.value;
		cssNode.MaxWidth = style.maxWidth.GetSpecifiedValueOrDefault(float.NaN);
		cssNode.MaxHeight = style.maxHeight.GetSpecifiedValueOrDefault(float.NaN);
		cssNode.MinWidth = style.minWidth.GetSpecifiedValueOrDefault(float.NaN);
		cssNode.MinHeight = style.minHeight.GetSpecifiedValueOrDefault(float.NaN);
		cssNode.FlexDirection = (CSSFlexDirection)style.flexDirection.value;
		cssNode.AlignContent = (CSSAlign)style.alignContent.GetSpecifiedValueOrDefault(Align.FlexStart);
		cssNode.AlignItems = (CSSAlign)style.alignItems.GetSpecifiedValueOrDefault(Align.Stretch);
		cssNode.JustifyContent = (CSSJustify)style.justifyContent.value;
		cssNode.Wrap = (CSSWrap)style.flexWrap.value;
		Dirty(ChangeType.Layout);
		Dirty(ChangeType.Transform);
	}

	internal void SetInlineStyles(VisualElementStylesData inlineStyle)
	{
		Debug.Assert(!inlineStyle.isShared);
		inlineStyle.Apply(m_Style, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity);
		m_Style = inlineStyle;
	}

	internal void SetSharedStyles(VisualElementStylesData sharedStyle)
	{
		Debug.Assert(sharedStyle.isShared);
		ClearDirty(ChangeType.Styles | ChangeType.StylesPath);
		if (sharedStyle != m_SharedStyle)
		{
			if (hasInlineStyle)
			{
				m_Style.Apply(sharedStyle, StylePropertyApplyMode.CopyIfNotInline);
			}
			else
			{
				m_Style = sharedStyle;
			}
			m_SharedStyle = sharedStyle;
			if (this.onStylesResolved != null)
			{
				this.onStylesResolved(m_Style);
			}
			OnStyleResolved(m_Style);
			Dirty(ChangeType.Repaint);
		}
	}

	public void ResetPositionProperties()
	{
		if (hasInlineStyle)
		{
			VisualElementStylesData visualElementStylesData = inlineStyle;
			visualElementStylesData.positionType = StyleValue<int>.nil;
			visualElementStylesData.marginLeft = StyleValue<float>.nil;
			visualElementStylesData.marginRight = StyleValue<float>.nil;
			visualElementStylesData.marginBottom = StyleValue<float>.nil;
			visualElementStylesData.marginTop = StyleValue<float>.nil;
			visualElementStylesData.positionLeft = StyleValue<float>.nil;
			visualElementStylesData.positionTop = StyleValue<float>.nil;
			visualElementStylesData.positionRight = StyleValue<float>.nil;
			visualElementStylesData.positionBottom = StyleValue<float>.nil;
			visualElementStylesData.width = StyleValue<float>.nil;
			visualElementStylesData.height = StyleValue<float>.nil;
			m_Style.Apply(sharedStyle, StylePropertyApplyMode.CopyIfNotInline);
			FinalizeLayout();
			Dirty(ChangeType.Layout);
		}
	}

	public override string ToString()
	{
		return string.Concat(GetType().Name, " ", name, " ", layout, " world rect: ", worldBound);
	}

	internal IEnumerable<string> GetClasses()
	{
		return m_ClassList;
	}

	public void ClearClassList()
	{
		if (m_ClassList != null && m_ClassList.Count > 0)
		{
			m_ClassList.Clear();
			Dirty(ChangeType.Styles);
		}
	}

	public void AddToClassList(string className)
	{
		if (m_ClassList == null)
		{
			m_ClassList = new HashSet<string>();
		}
		if (m_ClassList.Add(className))
		{
			Dirty(ChangeType.Styles);
		}
	}

	public void RemoveFromClassList(string className)
	{
		if (m_ClassList != null && m_ClassList.Remove(className))
		{
			Dirty(ChangeType.Styles);
		}
	}

	public bool ClassListContains(string cls)
	{
		return m_ClassList != null && m_ClassList.Contains(cls);
	}

	/// <summary>
	///   <para>Searchs up the hierachy of this VisualElement and retrieves stored userData, if any is found.</para>
	/// </summary>
	public object FindAncestorUserData()
	{
		for (VisualElement visualElement = parent; visualElement != null; visualElement = visualElement.parent)
		{
			if (visualElement.userData != null)
			{
				return visualElement.userData;
			}
		}
		return null;
	}

	private void UpdateCursorStyle(long eventType)
	{
		if (elementPanel != null)
		{
			if (eventType == EventBase<MouseOverEvent>.TypeId())
			{
				elementPanel.cursorManager.SetCursor(style.cursor.value);
			}
			else
			{
				elementPanel.cursorManager.ResetCursor();
			}
		}
	}

	IUIElementDataWatchRequest IUIElementDataWatch.RegisterWatch(Object toWatch, Action<Object> watchNotification)
	{
		DataWatchRequest dataWatchRequest = new DataWatchRequest(this);
		dataWatchRequest.notification = watchNotification;
		dataWatchRequest.watchedObject = toWatch;
		DataWatchRequest dataWatchRequest2 = dataWatchRequest;
		dataWatchRequest2.Start();
		return dataWatchRequest2;
	}

	void IUIElementDataWatch.UnregisterWatch(IUIElementDataWatchRequest requested)
	{
		if (requested is DataWatchRequest dataWatchRequest)
		{
			dataWatchRequest.Stop();
		}
	}

	/// <summary>
	///   <para>Add an element to this element's contentContainer</para>
	/// </summary>
	/// <param name="child"></param>
	public void Add(VisualElement child)
	{
		if (contentContainer == this)
		{
			shadow.Add(child);
		}
		else
		{
			contentContainer.Add(child);
		}
		child.m_LogicalParent = this;
	}

	public void Insert(int index, VisualElement element)
	{
		if (contentContainer == this)
		{
			shadow.Insert(index, element);
		}
		else
		{
			contentContainer.Insert(index, element);
		}
		element.m_LogicalParent = this;
	}

	/// <summary>
	///   <para>Removes this child from the hierarchy</para>
	/// </summary>
	/// <param name="element"></param>
	public void Remove(VisualElement element)
	{
		if (contentContainer == this)
		{
			shadow.Remove(element);
		}
		else
		{
			contentContainer.Remove(element);
		}
	}

	/// <summary>
	///   <para>Remove the child element located at this position from this element's contentContainer</para>
	/// </summary>
	/// <param name="index"></param>
	public void RemoveAt(int index)
	{
		if (contentContainer == this)
		{
			shadow.RemoveAt(index);
		}
		else
		{
			contentContainer.RemoveAt(index);
		}
	}

	/// <summary>
	///   <para>Remove all child elements from this element's contentContainer</para>
	/// </summary>
	public void Clear()
	{
		if (contentContainer == this)
		{
			shadow.Clear();
		}
		else
		{
			contentContainer.Clear();
		}
	}

	/// <summary>
	///   <para>Retrieves the child element at position</para>
	/// </summary>
	/// <param name="index"></param>
	public VisualElement ElementAt(int index)
	{
		if (contentContainer == this)
		{
			return shadow.ElementAt(index);
		}
		return contentContainer.ElementAt(index);
	}

	/// <summary>
	///   <para>Returns the elements from its contentContainer</para>
	/// </summary>
	public IEnumerable<VisualElement> Children()
	{
		if (contentContainer == this)
		{
			return shadow.Children();
		}
		return contentContainer.Children();
	}

	public void Sort(Comparison<VisualElement> comp)
	{
		if (contentContainer == this)
		{
			shadow.Sort(comp);
		}
		else
		{
			contentContainer.Sort(comp);
		}
	}

	/// <summary>
	///   <para>Brings this element to the end of its parent children list. The element will be visually in front of any overlapping sibling elements.</para>
	/// </summary>
	public void BringToFront()
	{
		if (shadow.parent != null)
		{
			shadow.parent.shadow.BringToFront(this);
		}
	}

	/// <summary>
	///   <para>Sends this element to the beginning of its parent children list. The element will be visually behind any overlapping sibling elements.</para>
	/// </summary>
	public void SendToBack()
	{
		if (shadow.parent != null)
		{
			shadow.parent.shadow.SendToBack(this);
		}
	}

	/// <summary>
	///   <para>Places this element right before the sibling element in their parent children list. If the element and the sibling position overlap, the element will be visually behind of its sibling.</para>
	/// </summary>
	/// <param name="sibling">The sibling element.</param>
	public void PlaceBehind(VisualElement sibling)
	{
		if (shadow.parent == null || sibling.shadow.parent != shadow.parent)
		{
			throw new ArgumentException("VisualElements are not siblings");
		}
		shadow.parent.shadow.PlaceBehind(this, sibling);
	}

	/// <summary>
	///   <para>Places this element right after the sibling element in their parent children list. If the element and the sibling position overlap, the element will be visually in front of its sibling.</para>
	/// </summary>
	/// <param name="sibling">The sibling element.</param>
	public void PlaceInFront(VisualElement sibling)
	{
		if (shadow.parent == null || sibling.shadow.parent != shadow.parent)
		{
			throw new ArgumentException("VisualElements are not siblings");
		}
		shadow.parent.shadow.PlaceInFront(this, sibling);
	}

	/// <summary>
	///   <para>Removes this element from its parent hierarchy</para>
	/// </summary>
	public void RemoveFromHierarchy()
	{
		if (shadow.parent != null)
		{
			shadow.parent.shadow.Remove(this);
		}
	}

	public T GetFirstOfType<T>() where T : class
	{
		if (this is T result)
		{
			return result;
		}
		return GetFirstAncestorOfType<T>();
	}

	public T GetFirstAncestorOfType<T>() where T : class
	{
		for (VisualElement visualElement = shadow.parent; visualElement != null; visualElement = visualElement.shadow.parent)
		{
			if (visualElement is T result)
			{
				return result;
			}
		}
		return (T)null;
	}

	/// <summary>
	///   <para>Returns true if the element is a direct child of this VisualElement</para>
	/// </summary>
	/// <param name="child"></param>
	public bool Contains(VisualElement child)
	{
		while (child != null)
		{
			if (child.shadow.parent == this)
			{
				return true;
			}
			child = child.shadow.parent;
		}
		return false;
	}

	/// <summary>
	///   <para>Finds the lowest commont ancestor between two VisualElements inside the VisualTree hierarchy</para>
	/// </summary>
	/// <param name="other"></param>
	public VisualElement FindCommonAncestor(VisualElement other)
	{
		if (panel != other.panel)
		{
			return null;
		}
		VisualElement visualElement = this;
		int num = 0;
		while (visualElement != null)
		{
			num++;
			visualElement = visualElement.shadow.parent;
		}
		VisualElement visualElement2 = other;
		int num2 = 0;
		while (visualElement2 != null)
		{
			num2++;
			visualElement2 = visualElement2.shadow.parent;
		}
		visualElement = this;
		visualElement2 = other;
		while (num > num2)
		{
			num--;
			visualElement = visualElement.shadow.parent;
		}
		while (num2 > num)
		{
			num2--;
			visualElement2 = visualElement2.shadow.parent;
		}
		while (visualElement != visualElement2)
		{
			visualElement = visualElement.shadow.parent;
			visualElement2 = visualElement2.shadow.parent;
		}
		return visualElement;
	}

	/// <summary>
	///   <para>Allows to iterate into this elements children</para>
	/// </summary>
	public IEnumerator<VisualElement> GetEnumerator()
	{
		if (contentContainer == this)
		{
			return shadow.Children().GetEnumerator();
		}
		return contentContainer.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		if (contentContainer == this)
		{
			return ((IEnumerable)shadow.Children()).GetEnumerator();
		}
		return ((IEnumerable)contentContainer).GetEnumerator();
	}

	IVisualElementScheduledItem IVisualElementScheduler.Execute(Action<TimerState> timerUpdateEvent)
	{
		TimerStateScheduledItem timerStateScheduledItem = new TimerStateScheduledItem(this, timerUpdateEvent);
		timerStateScheduledItem.timerUpdateStopCondition = ScheduledItem.OnceCondition;
		TimerStateScheduledItem timerStateScheduledItem2 = timerStateScheduledItem;
		timerStateScheduledItem2.Resume();
		return timerStateScheduledItem2;
	}

	IVisualElementScheduledItem IVisualElementScheduler.Execute(Action updateEvent)
	{
		SimpleScheduledItem simpleScheduledItem = new SimpleScheduledItem(this, updateEvent);
		simpleScheduledItem.timerUpdateStopCondition = ScheduledItem.OnceCondition;
		SimpleScheduledItem simpleScheduledItem2 = simpleScheduledItem;
		simpleScheduledItem2.Resume();
		return simpleScheduledItem2;
	}

	/// <summary>
	///   <para>Adds this stylesheet file to this element list of applied styles</para>
	/// </summary>
	/// <param name="sheetPath"></param>
	public void AddStyleSheetPath(string sheetPath)
	{
		if (m_StyleSheetPaths == null)
		{
			m_StyleSheetPaths = new List<string>();
		}
		m_StyleSheetPaths.Add(sheetPath);
		m_StyleSheets = null;
		Dirty(ChangeType.Styles);
	}

	/// <summary>
	///   <para>Removes this stylesheet file from this element list of applied styles</para>
	/// </summary>
	/// <param name="sheetPath"></param>
	public void RemoveStyleSheetPath(string sheetPath)
	{
		if (m_StyleSheetPaths == null)
		{
			Debug.LogWarning("Attempting to remove from null style sheet path list");
			return;
		}
		m_StyleSheetPaths.Remove(sheetPath);
		m_StyleSheets = null;
		Dirty(ChangeType.Styles);
	}

	public bool HasStyleSheetPath(string sheetPath)
	{
		return m_StyleSheetPaths != null && m_StyleSheetPaths.Contains(sheetPath);
	}

	internal void ReplaceStyleSheetPath(string oldSheetPath, string newSheetPath)
	{
		if (m_StyleSheetPaths == null)
		{
			Debug.LogWarning("Attempting to replace a style from null style sheet path list");
			return;
		}
		int num = m_StyleSheetPaths.IndexOf(oldSheetPath);
		if (num >= 0)
		{
			m_StyleSheetPaths[num] = newSheetPath;
			m_StyleSheets = null;
			Dirty(ChangeType.Styles);
		}
	}

	internal void LoadStyleSheetsFromPaths()
	{
		if (m_StyleSheetPaths == null || elementPanel == null)
		{
			return;
		}
		m_StyleSheets = new List<StyleSheet>();
		foreach (string styleSheetPath in m_StyleSheetPaths)
		{
			StyleSheet styleSheet = Panel.loadResourceFunc(styleSheetPath, typeof(StyleSheet)) as StyleSheet;
			if (styleSheet != null)
			{
				int i = 0;
				for (int num = styleSheet.complexSelectors.Length; i < num; i++)
				{
					styleSheet.complexSelectors[i].CachePseudoStateMasks();
				}
				m_StyleSheets.Add(styleSheet);
			}
			else
			{
				Debug.LogWarning($"Style sheet not found for path \"{styleSheetPath}\"");
			}
		}
	}
}
