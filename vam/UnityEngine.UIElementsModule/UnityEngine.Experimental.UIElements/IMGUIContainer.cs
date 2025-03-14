#define UNITY_ASSERTIONS
using System;

namespace UnityEngine.Experimental.UIElements;

public class IMGUIContainer : VisualElement
{
	private struct GUIGlobals
	{
		public Matrix4x4 matrix;

		public Color color;

		public Color contentColor;

		public Color backgroundColor;

		public bool enabled;

		public bool changed;

		public int displayIndex;
	}

	private readonly Action m_OnGUIHandler;

	private ObjectGUIState m_ObjectGUIState;

	internal bool useOwnerObjectGUIState;

	private GUILayoutUtility.LayoutCache m_Cache = null;

	private bool lostFocus = false;

	private bool receivedFocus = false;

	private FocusChangeDirection focusChangeDirection = FocusChangeDirection.unspecified;

	private bool hasFocusableControls = false;

	private int newKeyboardFocusControlID = 0;

	private GUIGlobals m_GUIGlobals;

	internal ObjectGUIState guiState
	{
		get
		{
			Debug.Assert(!useOwnerObjectGUIState);
			if (m_ObjectGUIState == null)
			{
				m_ObjectGUIState = new ObjectGUIState();
			}
			return m_ObjectGUIState;
		}
	}

	internal Rect lastWorldClip { get; set; }

	private GUILayoutUtility.LayoutCache cache
	{
		get
		{
			if (m_Cache == null)
			{
				m_Cache = new GUILayoutUtility.LayoutCache();
			}
			return m_Cache;
		}
	}

	public ContextType contextType { get; set; }

	internal int GUIDepth { get; private set; }

	public override bool canGrabFocus => base.canGrabFocus && hasFocusableControls;

	public IMGUIContainer(Action onGUIHandler)
	{
		m_OnGUIHandler = onGUIHandler;
		contextType = ContextType.Editor;
		base.focusIndex = 0;
	}

	internal override void DoRepaint(IStylePainter painter)
	{
		base.DoRepaint();
		lastWorldClip = painter.currentWorldClip;
		HandleIMGUIEvent(painter.repaintEvent);
	}

	internal override void ChangePanel(BaseVisualElementPanel p)
	{
		if (base.elementPanel != null)
		{
			base.elementPanel.IMGUIContainersCount--;
		}
		base.ChangePanel(p);
		if (base.elementPanel != null)
		{
			base.elementPanel.IMGUIContainersCount++;
		}
	}

	private void SaveGlobals()
	{
		m_GUIGlobals.matrix = GUI.matrix;
		m_GUIGlobals.color = GUI.color;
		m_GUIGlobals.contentColor = GUI.contentColor;
		m_GUIGlobals.backgroundColor = GUI.backgroundColor;
		m_GUIGlobals.enabled = GUI.enabled;
		m_GUIGlobals.changed = GUI.changed;
		if (Event.current != null)
		{
			m_GUIGlobals.displayIndex = Event.current.displayIndex;
		}
	}

	private void RestoreGlobals()
	{
		GUI.matrix = m_GUIGlobals.matrix;
		GUI.color = m_GUIGlobals.color;
		GUI.contentColor = m_GUIGlobals.contentColor;
		GUI.backgroundColor = m_GUIGlobals.backgroundColor;
		GUI.enabled = m_GUIGlobals.enabled;
		GUI.changed = m_GUIGlobals.changed;
		if (Event.current != null)
		{
			Event.current.displayIndex = m_GUIGlobals.displayIndex;
		}
	}

	private void DoOnGUI(Event evt)
	{
		if (m_OnGUIHandler == null || base.panel == null)
		{
			return;
		}
		int num = GUIClip.Internal_GetCount();
		SaveGlobals();
		UIElementsUtility.BeginContainerGUI(cache, evt, this);
		if (evt.type != EventType.Layout)
		{
			if (lostFocus)
			{
				if (focusController != null && (focusController.focusedElement == null || focusController.focusedElement == this || !(focusController.focusedElement is IMGUIContainer)))
				{
					GUIUtility.keyboardControl = 0;
					focusController.imguiKeyboardControl = 0;
				}
				lostFocus = false;
			}
			if (receivedFocus)
			{
				if (focusChangeDirection != FocusChangeDirection.unspecified && focusChangeDirection != FocusChangeDirection.none)
				{
					if (focusChangeDirection == VisualElementFocusChangeDirection.left)
					{
						GUIUtility.SetKeyboardControlToLastControlId();
					}
					else if (focusChangeDirection == VisualElementFocusChangeDirection.right)
					{
						GUIUtility.SetKeyboardControlToFirstControlId();
					}
				}
				receivedFocus = false;
				focusChangeDirection = FocusChangeDirection.unspecified;
				if (focusController != null)
				{
					focusController.imguiKeyboardControl = GUIUtility.keyboardControl;
				}
			}
		}
		GUIDepth = GUIUtility.Internal_GetGUIDepth();
		EventType type = Event.current.type;
		bool flag = false;
		try
		{
			if (type != EventType.Layout)
			{
				GetCurrentTransformAndClip(this, evt, out var objectTransform, out var clipRect);
				using (new GUIClip.ParentClipScope(objectTransform, clipRect))
				{
					m_OnGUIHandler();
				}
			}
			else
			{
				m_OnGUIHandler();
			}
		}
		catch (Exception exception)
		{
			if (type != EventType.Layout)
			{
				throw;
			}
			flag = GUIUtility.IsExitGUIException(exception);
			if (!flag)
			{
				Debug.LogException(exception);
			}
		}
		finally
		{
			if (evt.type != EventType.Layout)
			{
				int keyboardControl = GUIUtility.keyboardControl;
				int num2 = GUIUtility.CheckForTabEvent(evt);
				if (focusController != null)
				{
					if (num2 < 0)
					{
						Focusable focusedElement = focusController.focusedElement;
						using (KeyDownEvent e = KeyboardEventBase<KeyDownEvent>.GetPooled('\t', KeyCode.Tab, (num2 != -1) ? EventModifiers.Shift : EventModifiers.None))
						{
							focusController.SwitchFocusOnEvent(e);
						}
						if (focusedElement == this)
						{
							if (focusController.focusedElement == this)
							{
								switch (num2)
								{
								case -2:
									GUIUtility.SetKeyboardControlToLastControlId();
									break;
								case -1:
									GUIUtility.SetKeyboardControlToFirstControlId();
									break;
								}
								newKeyboardFocusControlID = GUIUtility.keyboardControl;
								focusController.imguiKeyboardControl = GUIUtility.keyboardControl;
							}
							else
							{
								GUIUtility.keyboardControl = 0;
								focusController.imguiKeyboardControl = 0;
							}
						}
					}
					else if (num2 > 0)
					{
						focusController.imguiKeyboardControl = GUIUtility.keyboardControl;
						newKeyboardFocusControlID = GUIUtility.keyboardControl;
					}
					else if (num2 == 0 && (keyboardControl != GUIUtility.keyboardControl || type == EventType.MouseDown))
					{
						focusController.SyncIMGUIFocus(GUIUtility.keyboardControl, this);
					}
				}
				hasFocusableControls = GUIUtility.HasFocusableControls();
			}
		}
		EventType type2 = Event.current.type;
		UIElementsUtility.EndContainerGUI();
		RestoreGlobals();
		if (!flag && type2 != EventType.Ignore && type2 != EventType.Used)
		{
			int num3 = GUIClip.Internal_GetCount();
			if (num3 > num)
			{
				Debug.LogError("GUI Error: You are pushing more GUIClips than you are popping. Make sure they are balanced)");
			}
			else if (num3 < num)
			{
				Debug.LogError("GUI Error: You are popping more GUIClips than you are pushing. Make sure they are balanced)");
			}
		}
		while (GUIClip.Internal_GetCount() > num)
		{
			GUIClip.Internal_Pop();
		}
		if (type2 == EventType.Used)
		{
			Dirty(ChangeType.Repaint);
		}
	}

	public override void HandleEvent(EventBase evt)
	{
		base.HandleEvent(evt);
		if (evt.propagationPhase != PropagationPhase.DefaultAction && evt.imguiEvent != null && !evt.isPropagationStopped && m_OnGUIHandler != null && base.elementPanel != null && base.elementPanel.IMGUIEventInterests.WantsEvent(evt.imguiEvent.type) && HandleIMGUIEvent(evt.imguiEvent))
		{
			evt.StopPropagation();
			evt.PreventDefault();
		}
	}

	internal bool HandleIMGUIEvent(Event e)
	{
		EventType type = e.type;
		e.type = EventType.Layout;
		DoOnGUI(e);
		e.type = type;
		DoOnGUI(e);
		if (newKeyboardFocusControlID > 0)
		{
			newKeyboardFocusControlID = 0;
			Event @event = new Event();
			@event.type = EventType.ExecuteCommand;
			@event.commandName = "NewKeyboardFocus";
			HandleIMGUIEvent(@event);
		}
		if (e.type == EventType.Used)
		{
			return true;
		}
		if (e.type == EventType.MouseUp && this.HasMouseCapture())
		{
			GUIUtility.hotControl = 0;
		}
		if (base.elementPanel == null)
		{
			GUIUtility.ExitGUI();
		}
		return false;
	}

	protected internal override void ExecuteDefaultAction(EventBase evt)
	{
		if (evt.GetEventTypeId() == EventBase<BlurEvent>.TypeId())
		{
			BlurEvent blurEvent = evt as BlurEvent;
			if (blurEvent.relatedTarget is VisualElement visualElement && (blurEvent.relatedTarget.canGrabFocus || visualElement.parent == base.panel.visualTree))
			{
				lostFocus = true;
			}
		}
		else if (evt.GetEventTypeId() == EventBase<FocusEvent>.TypeId())
		{
			FocusEvent focusEvent = evt as FocusEvent;
			receivedFocus = true;
			focusChangeDirection = focusEvent.direction;
		}
	}

	protected internal override Vector2 DoMeasure(float desiredWidth, MeasureMode widthMode, float desiredHeight, MeasureMode heightMode)
	{
		float num = float.NaN;
		float num2 = float.NaN;
		if (widthMode != MeasureMode.Exactly || heightMode != MeasureMode.Exactly)
		{
			DoOnGUI(new Event
			{
				type = EventType.Layout
			});
			num = m_Cache.topLevel.minWidth;
			num2 = m_Cache.topLevel.minHeight;
		}
		switch (widthMode)
		{
		case MeasureMode.Exactly:
			num = desiredWidth;
			break;
		case MeasureMode.AtMost:
			num = Mathf.Min(num, desiredWidth);
			break;
		}
		switch (heightMode)
		{
		case MeasureMode.Exactly:
			num2 = desiredHeight;
			break;
		case MeasureMode.AtMost:
			num2 = Mathf.Min(num2, desiredHeight);
			break;
		}
		return new Vector2(num, num2);
	}

	private static void GetCurrentTransformAndClip(IMGUIContainer container, Event evt, out Matrix4x4 transform, out Rect clipRect)
	{
		clipRect = container.lastWorldClip;
		if (clipRect.width == 0f || clipRect.height == 0f)
		{
			clipRect = container.worldBound;
		}
		transform = container.worldTransform;
		if (evt.type == EventType.Repaint && container.elementPanel != null && container.elementPanel.stylePainter != null)
		{
			transform = container.elementPanel.stylePainter.currentTransform;
		}
	}
}
