namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>The base class for mouse events.</para>
/// </summary>
public abstract class MouseEventBase<T> : EventBase<T>, IMouseEvent where T : MouseEventBase<T>, new()
{
	public EventModifiers modifiers { get; protected set; }

	public Vector2 mousePosition { get; protected set; }

	public Vector2 localMousePosition { get; internal set; }

	public Vector2 mouseDelta { get; protected set; }

	public int clickCount { get; protected set; }

	public int button { get; protected set; }

	public bool shiftKey => (modifiers & EventModifiers.Shift) != 0;

	public bool ctrlKey => (modifiers & EventModifiers.Control) != 0;

	public bool commandKey => (modifiers & EventModifiers.Command) != 0;

	public bool altKey => (modifiers & EventModifiers.Alt) != 0;

	public override IEventHandler currentTarget
	{
		get
		{
			return base.currentTarget;
		}
		internal set
		{
			base.currentTarget = value;
			if (currentTarget is VisualElement ele)
			{
				localMousePosition = ele.WorldToLocal(mousePosition);
			}
		}
	}

	protected MouseEventBase()
	{
		Init();
	}

	protected override void Init()
	{
		base.Init();
		base.flags = EventFlags.Bubbles | EventFlags.Capturable | EventFlags.Cancellable;
		modifiers = EventModifiers.None;
		mousePosition = Vector2.zero;
		localMousePosition = Vector2.zero;
		mouseDelta = Vector2.zero;
		clickCount = 0;
		button = 0;
	}

	public static T GetPooled(Event systemEvent)
	{
		T pooled = EventBase<T>.GetPooled();
		pooled.imguiEvent = systemEvent;
		if (systemEvent != null)
		{
			pooled.modifiers = systemEvent.modifiers;
			pooled.mousePosition = systemEvent.mousePosition;
			pooled.localMousePosition = systemEvent.mousePosition;
			pooled.mouseDelta = systemEvent.delta;
			pooled.button = systemEvent.button;
			pooled.clickCount = systemEvent.clickCount;
		}
		return pooled;
	}

	public static T GetPooled(IMouseEvent triggerEvent)
	{
		T pooled = EventBase<T>.GetPooled();
		if (triggerEvent != null)
		{
			pooled.modifiers = triggerEvent.modifiers;
			pooled.mousePosition = triggerEvent.mousePosition;
			pooled.localMousePosition = triggerEvent.mousePosition;
			pooled.mouseDelta = triggerEvent.mouseDelta;
			pooled.button = triggerEvent.button;
			pooled.clickCount = triggerEvent.clickCount;
		}
		return pooled;
	}
}
