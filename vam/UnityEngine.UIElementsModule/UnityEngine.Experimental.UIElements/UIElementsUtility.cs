#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements;

internal class UIElementsUtility
{
	private static Stack<IMGUIContainer> s_ContainerStack;

	private static Dictionary<int, Panel> s_UIElementsCache;

	private static Event s_EventInstance;

	private static EventDispatcher s_EventDispatcher;

	internal static IEventDispatcher eventDispatcher
	{
		get
		{
			if (s_EventDispatcher == null)
			{
				s_EventDispatcher = new EventDispatcher();
			}
			return s_EventDispatcher;
		}
	}

	static UIElementsUtility()
	{
		s_ContainerStack = new Stack<IMGUIContainer>();
		s_UIElementsCache = new Dictionary<int, Panel>();
		s_EventInstance = new Event();
		GUIUtility.takeCapture = (Action)Delegate.Combine(GUIUtility.takeCapture, new Action(TakeCapture));
		GUIUtility.releaseCapture = (Action)Delegate.Combine(GUIUtility.releaseCapture, new Action(ReleaseCapture));
		GUIUtility.processEvent = (Func<int, IntPtr, bool>)Delegate.Combine(GUIUtility.processEvent, new Func<int, IntPtr, bool>(ProcessEvent));
		GUIUtility.cleanupRoots = (Action)Delegate.Combine(GUIUtility.cleanupRoots, new Action(CleanupRoots));
		GUIUtility.endContainerGUIFromException = (Func<Exception, bool>)Delegate.Combine(GUIUtility.endContainerGUIFromException, new Func<Exception, bool>(EndContainerGUIFromException));
	}

	internal static void ClearDispatcher()
	{
		s_EventDispatcher = null;
	}

	private static void TakeCapture()
	{
		if (s_ContainerStack.Count <= 0)
		{
			return;
		}
		IMGUIContainer iMGUIContainer = s_ContainerStack.Peek();
		if (iMGUIContainer.GUIDepth == GUIUtility.Internal_GetGUIDepth())
		{
			if (MouseCaptureController.IsMouseCaptureTaken() && !iMGUIContainer.HasMouseCapture())
			{
				Debug.Log("Should not grab hot control with an active capture");
			}
			iMGUIContainer.TakeMouseCapture();
		}
	}

	private static void ReleaseCapture()
	{
		MouseCaptureController.ReleaseMouseCapture();
	}

	private static bool ProcessEvent(int instanceID, IntPtr nativeEventPtr)
	{
		if (nativeEventPtr != IntPtr.Zero && s_UIElementsCache.TryGetValue(instanceID, out var value))
		{
			s_EventInstance.CopyFromPtr(nativeEventPtr);
			return DoDispatch(value);
		}
		return false;
	}

	public static void RemoveCachedPanel(int instanceID)
	{
		s_UIElementsCache.Remove(instanceID);
	}

	private static void CleanupRoots()
	{
		s_EventInstance = null;
		s_EventDispatcher = null;
		s_UIElementsCache = null;
		s_ContainerStack = null;
	}

	private static bool EndContainerGUIFromException(Exception exception)
	{
		if (s_ContainerStack.Count > 0)
		{
			GUIUtility.EndContainer();
			s_ContainerStack.Pop();
		}
		return GUIUtility.ShouldRethrowException(exception);
	}

	internal static void BeginContainerGUI(GUILayoutUtility.LayoutCache cache, Event evt, IMGUIContainer container)
	{
		if (container.useOwnerObjectGUIState)
		{
			GUIUtility.BeginContainerFromOwner(container.elementPanel.ownerObject);
		}
		else
		{
			GUIUtility.BeginContainer(container.guiState);
		}
		s_ContainerStack.Push(container);
		GUIUtility.s_SkinMode = (int)container.contextType;
		GUIUtility.s_OriginalID = container.elementPanel.ownerObject.GetInstanceID();
		Event.current = evt;
		GUI.enabled = container.enabledInHierarchy;
		GUILayoutUtility.BeginContainer(cache);
		GUIUtility.ResetGlobalState();
	}

	internal static void EndContainerGUI()
	{
		if (Event.current.type == EventType.Layout && s_ContainerStack.Count > 0)
		{
			Rect layout = s_ContainerStack.Peek().layout;
			GUILayoutUtility.LayoutFromContainer(layout.width, layout.height);
		}
		GUILayoutUtility.SelectIDList(GUIUtility.s_OriginalID, isWindow: false);
		GUIContent.ClearStaticCache();
		if (s_ContainerStack.Count > 0)
		{
			GUIUtility.EndContainer();
			s_ContainerStack.Pop();
		}
	}

	internal static ContextType GetGUIContextType()
	{
		return (GUIUtility.s_SkinMode != 0) ? ContextType.Editor : ContextType.Player;
	}

	internal static EventBase CreateEvent(Event systemEvent)
	{
		return systemEvent.type switch
		{
			EventType.MouseMove => MouseEventBase<MouseMoveEvent>.GetPooled(systemEvent), 
			EventType.MouseDrag => MouseEventBase<MouseMoveEvent>.GetPooled(systemEvent), 
			EventType.MouseDown => MouseEventBase<MouseDownEvent>.GetPooled(systemEvent), 
			EventType.MouseUp => MouseEventBase<MouseUpEvent>.GetPooled(systemEvent), 
			EventType.ScrollWheel => WheelEvent.GetPooled(systemEvent), 
			EventType.KeyDown => KeyboardEventBase<KeyDownEvent>.GetPooled(systemEvent), 
			EventType.KeyUp => KeyboardEventBase<KeyUpEvent>.GetPooled(systemEvent), 
			EventType.MouseEnterWindow => MouseEventBase<MouseEnterWindowEvent>.GetPooled(systemEvent), 
			EventType.MouseLeaveWindow => MouseEventBase<MouseLeaveWindowEvent>.GetPooled(systemEvent), 
			_ => IMGUIEvent.GetPooled(systemEvent), 
		};
	}

	private static bool DoDispatch(BaseVisualElementPanel panel)
	{
		if (s_EventInstance.type == EventType.Repaint)
		{
			bool sRGBWrite = GL.sRGBWrite;
			if (sRGBWrite)
			{
				GL.sRGBWrite = false;
			}
			panel.Repaint(s_EventInstance);
			if (sRGBWrite)
			{
				GL.sRGBWrite = true;
			}
			return panel.IMGUIContainersCount > 0;
		}
		panel.ValidateLayout();
		using EventBase eventBase = CreateEvent(s_EventInstance);
		s_EventDispatcher.DispatchEvent(eventBase, panel);
		s_EventInstance.mousePosition = eventBase.originalMousePosition;
		if (eventBase.isPropagationStopped)
		{
			panel.visualTree.Dirty(ChangeType.Repaint);
		}
		return eventBase.isPropagationStopped;
	}

	internal static Dictionary<int, Panel>.Enumerator GetPanelsIterator()
	{
		return s_UIElementsCache.GetEnumerator();
	}

	internal static Panel FindOrCreatePanel(ScriptableObject ownerObject, ContextType contextType, IDataWatchService dataWatch = null)
	{
		if (!s_UIElementsCache.TryGetValue(ownerObject.GetInstanceID(), out var value))
		{
			value = new Panel(ownerObject, contextType, dataWatch, eventDispatcher);
			s_UIElementsCache.Add(ownerObject.GetInstanceID(), value);
		}
		else
		{
			Debug.Assert(contextType == value.contextType, "Context type mismatch");
		}
		return value;
	}

	internal static Panel FindOrCreatePanel(ScriptableObject ownerObject)
	{
		return FindOrCreatePanel(ownerObject, GetGUIContextType());
	}
}
