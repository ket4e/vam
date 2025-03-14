#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements;

internal class EventDispatcher : IEventDispatcher
{
	private class PropagationPaths : IDisposable
	{
		[Flags]
		public enum Type
		{
			None = 0,
			Capture = 1,
			BubbleUp = 2
		}

		public readonly List<VisualElement> capturePath;

		public readonly List<VisualElement> bubblePath;

		public PropagationPaths(int initialSize)
		{
			capturePath = new List<VisualElement>(initialSize);
			bubblePath = new List<VisualElement>(initialSize);
		}

		public void Dispose()
		{
			PropagationPathsPool.Release(this);
		}

		public void Clear()
		{
			bubblePath.Clear();
			capturePath.Clear();
		}
	}

	private static class PropagationPathsPool
	{
		private static readonly List<PropagationPaths> s_Available = new List<PropagationPaths>();

		public static PropagationPaths Acquire()
		{
			if (s_Available.Count != 0)
			{
				PropagationPaths result = s_Available[0];
				s_Available.RemoveAt(0);
				return result;
			}
			return new PropagationPaths(16);
		}

		public static void Release(PropagationPaths po)
		{
			po.Clear();
			s_Available.Add(po);
		}
	}

	private VisualElement m_TopElementUnderMouse;

	private const int k_DefaultPropagationDepth = 16;

	private void DispatchMouseEnterMouseLeave(VisualElement previousTopElementUnderMouse, VisualElement currentTopElementUnderMouse, Event triggerEvent)
	{
		if (previousTopElementUnderMouse == currentTopElementUnderMouse)
		{
			return;
		}
		int num = 0;
		VisualElement visualElement;
		for (visualElement = previousTopElementUnderMouse; visualElement != null; visualElement = visualElement.shadow.parent)
		{
			num++;
		}
		int num2 = 0;
		VisualElement visualElement2;
		for (visualElement2 = currentTopElementUnderMouse; visualElement2 != null; visualElement2 = visualElement2.shadow.parent)
		{
			num2++;
		}
		visualElement = previousTopElementUnderMouse;
		visualElement2 = currentTopElementUnderMouse;
		while (num > num2)
		{
			using (MouseLeaveEvent mouseLeaveEvent = MouseEventBase<MouseLeaveEvent>.GetPooled(triggerEvent))
			{
				mouseLeaveEvent.target = visualElement;
				DispatchEvent(mouseLeaveEvent, visualElement.panel);
			}
			num--;
			visualElement = visualElement.shadow.parent;
		}
		List<VisualElement> list = new List<VisualElement>(num2);
		while (num2 > num)
		{
			list.Add(visualElement2);
			num2--;
			visualElement2 = visualElement2.shadow.parent;
		}
		while (visualElement != visualElement2)
		{
			using (MouseLeaveEvent mouseLeaveEvent2 = MouseEventBase<MouseLeaveEvent>.GetPooled(triggerEvent))
			{
				mouseLeaveEvent2.target = visualElement;
				DispatchEvent(mouseLeaveEvent2, visualElement.panel);
			}
			list.Add(visualElement2);
			visualElement = visualElement.shadow.parent;
			visualElement2 = visualElement2.shadow.parent;
		}
		for (int num3 = list.Count - 1; num3 >= 0; num3--)
		{
			using MouseEnterEvent mouseEnterEvent = MouseEventBase<MouseEnterEvent>.GetPooled(triggerEvent);
			mouseEnterEvent.target = list[num3];
			DispatchEvent(mouseEnterEvent, list[num3].panel);
		}
	}

	private void DispatchMouseOverMouseOut(VisualElement previousTopElementUnderMouse, VisualElement currentTopElementUnderMouse, Event triggerEvent)
	{
		if (previousTopElementUnderMouse == currentTopElementUnderMouse)
		{
			return;
		}
		if (previousTopElementUnderMouse != null)
		{
			using MouseOutEvent mouseOutEvent = MouseEventBase<MouseOutEvent>.GetPooled(triggerEvent);
			mouseOutEvent.target = previousTopElementUnderMouse;
			DispatchEvent(mouseOutEvent, previousTopElementUnderMouse.panel);
		}
		if (currentTopElementUnderMouse == null)
		{
			return;
		}
		using MouseOverEvent mouseOverEvent = MouseEventBase<MouseOverEvent>.GetPooled(triggerEvent);
		mouseOverEvent.target = currentTopElementUnderMouse;
		DispatchEvent(mouseOverEvent, currentTopElementUnderMouse.panel);
	}

	public void DispatchEvent(EventBase evt, IPanel panel)
	{
		Event imguiEvent = evt.imguiEvent;
		if (imguiEvent != null && imguiEvent.type == EventType.Repaint)
		{
			return;
		}
		bool flag = false;
		VisualElement visualElement = null;
		if ((evt is IMouseEvent || imguiEvent != null) && MouseCaptureController.mouseCapture != null)
		{
			visualElement = MouseCaptureController.mouseCapture as VisualElement;
			if (visualElement != null && visualElement.panel == null)
			{
				Debug.Log(string.Format("Capture has no panel, forcing removal (capture={0} eventType={1})", MouseCaptureController.mouseCapture, (imguiEvent == null) ? "null" : imguiEvent.type.ToString()));
				MouseCaptureController.ReleaseMouseCapture();
				visualElement = null;
			}
			if (panel != null && visualElement != null && visualElement.panel.contextType != panel.contextType)
			{
				return;
			}
			flag = true;
			evt.dispatch = true;
			if (MouseCaptureController.mouseCapture != null)
			{
				evt.target = MouseCaptureController.mouseCapture;
				evt.currentTarget = MouseCaptureController.mouseCapture;
				evt.propagationPhase = PropagationPhase.AtTarget;
				MouseCaptureController.mouseCapture.HandleEvent(evt);
			}
			evt.propagationPhase = PropagationPhase.None;
			evt.currentTarget = null;
			evt.dispatch = false;
		}
		if (evt.isPropagationStopped)
		{
			if (evt.target == null && panel != null)
			{
				evt.target = panel.visualTree;
			}
			if (evt.target != null && evt.target != MouseCaptureController.mouseCapture)
			{
				evt.dispatch = true;
				evt.currentTarget = evt.target;
				evt.propagationPhase = PropagationPhase.AtTarget;
				evt.target.HandleEvent(evt);
				evt.propagationPhase = PropagationPhase.None;
				evt.currentTarget = null;
				evt.dispatch = false;
			}
		}
		if (!evt.isPropagationStopped)
		{
			if (evt is IKeyboardEvent && panel != null)
			{
				if (panel.focusController.focusedElement != null)
				{
					IMGUIContainer iMGUIContainer = panel.focusController.focusedElement as IMGUIContainer;
					flag = true;
					if (iMGUIContainer != null)
					{
						if (iMGUIContainer.HandleIMGUIEvent(evt.imguiEvent))
						{
							evt.StopPropagation();
							evt.PreventDefault();
						}
					}
					else
					{
						evt.target = panel.focusController.focusedElement;
						PropagateEvent(evt);
					}
				}
				else
				{
					evt.target = panel.visualTree;
					PropagateEvent(evt);
					flag = false;
				}
			}
			else if (evt.GetEventTypeId() == EventBase<MouseEnterEvent>.TypeId() || evt.GetEventTypeId() == EventBase<MouseLeaveEvent>.TypeId())
			{
				Debug.Assert(evt.target != null);
				flag = true;
				PropagateEvent(evt);
			}
			else if (evt is IMouseEvent || (imguiEvent != null && (imguiEvent.type == EventType.ContextClick || imguiEvent.type == EventType.DragUpdated || imguiEvent.type == EventType.DragPerform || imguiEvent.type == EventType.DragExited)))
			{
				VisualElement topElementUnderMouse = m_TopElementUnderMouse;
				if (evt.GetEventTypeId() == EventBase<MouseLeaveWindowEvent>.TypeId())
				{
					m_TopElementUnderMouse = null;
					DispatchMouseEnterMouseLeave(topElementUnderMouse, m_TopElementUnderMouse, imguiEvent);
					DispatchMouseOverMouseOut(topElementUnderMouse, m_TopElementUnderMouse, imguiEvent);
				}
				else if (evt is IMouseEvent || imguiEvent != null)
				{
					if (evt.target == null && panel != null)
					{
						if (evt is IMouseEvent)
						{
							m_TopElementUnderMouse = panel.Pick((evt as IMouseEvent).localMousePosition);
						}
						else if (imguiEvent != null)
						{
							m_TopElementUnderMouse = panel.Pick(imguiEvent.mousePosition);
						}
						evt.target = m_TopElementUnderMouse;
					}
					if (evt.target != null)
					{
						flag = true;
						PropagateEvent(evt);
					}
					if (evt.GetEventTypeId() == EventBase<MouseMoveEvent>.TypeId() || evt.GetEventTypeId() == EventBase<MouseEnterWindowEvent>.TypeId() || evt.GetEventTypeId() == EventBase<WheelEvent>.TypeId() || (imguiEvent != null && imguiEvent.type == EventType.DragUpdated))
					{
						DispatchMouseEnterMouseLeave(topElementUnderMouse, m_TopElementUnderMouse, imguiEvent);
						DispatchMouseOverMouseOut(topElementUnderMouse, m_TopElementUnderMouse, imguiEvent);
					}
				}
			}
			else if (panel != null && imguiEvent != null && (imguiEvent.type == EventType.ExecuteCommand || imguiEvent.type == EventType.ValidateCommand))
			{
				if (panel.focusController.focusedElement is IMGUIContainer iMGUIContainer2)
				{
					flag = true;
					if (iMGUIContainer2.HandleIMGUIEvent(evt.imguiEvent))
					{
						evt.StopPropagation();
						evt.PreventDefault();
					}
				}
				else if (panel.focusController.focusedElement != null)
				{
					flag = true;
					evt.target = panel.focusController.focusedElement;
					PropagateEvent(evt);
				}
				else
				{
					flag = true;
					PropagateToIMGUIContainer(panel.visualTree, evt, visualElement);
				}
			}
			else if (evt is IPropagatableEvent || evt is IFocusEvent || evt is IChangeEvent || evt.GetEventTypeId() == EventBase<InputEvent>.TypeId() || evt.GetEventTypeId() == EventBase<PostLayoutEvent>.TypeId())
			{
				Debug.Assert(evt.target != null);
				flag = true;
				PropagateEvent(evt);
			}
		}
		if (!evt.isPropagationStopped && imguiEvent != null && panel != null && (!flag || (imguiEvent != null && (imguiEvent.type == EventType.MouseEnterWindow || imguiEvent.type == EventType.MouseLeaveWindow || imguiEvent.type == EventType.Used))))
		{
			PropagateToIMGUIContainer(panel.visualTree, evt, visualElement);
		}
		if (evt.target == null && panel != null)
		{
			evt.target = panel.visualTree;
		}
		ExecuteDefaultAction(evt);
	}

	private static void PropagateToIMGUIContainer(VisualElement root, EventBase evt, VisualElement capture)
	{
		if (root is IMGUIContainer iMGUIContainer && (evt.imguiEvent.type == EventType.Used || root != capture))
		{
			if (iMGUIContainer.HandleIMGUIEvent(evt.imguiEvent))
			{
				evt.StopPropagation();
				evt.PreventDefault();
			}
		}
		else
		{
			if (root == null)
			{
				return;
			}
			for (int i = 0; i < root.shadow.childCount; i++)
			{
				PropagateToIMGUIContainer(root.shadow[i], evt, capture);
				if (evt.isPropagationStopped)
				{
					break;
				}
			}
		}
	}

	private static void PropagateEvent(EventBase evt)
	{
		if (evt.dispatch)
		{
			return;
		}
		PropagationPaths.Type type = (evt.capturable ? PropagationPaths.Type.Capture : PropagationPaths.Type.None);
		type |= (evt.bubbles ? PropagationPaths.Type.BubbleUp : PropagationPaths.Type.None);
		using PropagationPaths propagationPaths = BuildPropagationPath(evt.target as VisualElement, type);
		evt.dispatch = true;
		if (evt.capturable && propagationPaths != null && propagationPaths.capturePath.Count > 0)
		{
			evt.propagationPhase = PropagationPhase.Capture;
			int num = propagationPaths.capturePath.Count - 1;
			while (num >= 0 && !evt.isPropagationStopped)
			{
				evt.currentTarget = propagationPaths.capturePath[num];
				evt.currentTarget.HandleEvent(evt);
				num--;
			}
		}
		evt.propagationPhase = PropagationPhase.AtTarget;
		evt.currentTarget = evt.target;
		evt.currentTarget.HandleEvent(evt);
		if (evt.bubbles && propagationPaths != null && propagationPaths.bubblePath.Count > 0)
		{
			evt.propagationPhase = PropagationPhase.BubbleUp;
			foreach (VisualElement item in propagationPaths.bubblePath)
			{
				if (evt.isPropagationStopped)
				{
					break;
				}
				evt.currentTarget = item;
				evt.currentTarget.HandleEvent(evt);
			}
		}
		evt.dispatch = false;
		evt.propagationPhase = PropagationPhase.None;
		evt.currentTarget = null;
	}

	private static void ExecuteDefaultAction(EventBase evt)
	{
		if (evt.target != null)
		{
			evt.dispatch = true;
			evt.currentTarget = evt.target;
			evt.propagationPhase = PropagationPhase.DefaultAction;
			evt.currentTarget.HandleEvent(evt);
			evt.propagationPhase = PropagationPhase.None;
			evt.currentTarget = null;
			evt.dispatch = false;
		}
	}

	private static PropagationPaths BuildPropagationPath(VisualElement elem, PropagationPaths.Type pathTypesRequested)
	{
		if (elem == null || pathTypesRequested == PropagationPaths.Type.None)
		{
			return null;
		}
		PropagationPaths propagationPaths = PropagationPathsPool.Acquire();
		while (elem.shadow.parent != null)
		{
			if (elem.shadow.parent.enabledInHierarchy)
			{
				if ((pathTypesRequested & PropagationPaths.Type.Capture) == PropagationPaths.Type.Capture && elem.shadow.parent.HasCaptureHandlers())
				{
					propagationPaths.capturePath.Add(elem.shadow.parent);
				}
				if ((pathTypesRequested & PropagationPaths.Type.BubbleUp) == PropagationPaths.Type.BubbleUp && elem.shadow.parent.HasBubbleHandlers())
				{
					propagationPaths.bubblePath.Add(elem.shadow.parent);
				}
			}
			elem = elem.shadow.parent;
		}
		return propagationPaths;
	}
}
