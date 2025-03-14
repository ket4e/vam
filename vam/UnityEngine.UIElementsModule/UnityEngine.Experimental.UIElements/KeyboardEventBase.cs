namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Base class for keyboard events.</para>
/// </summary>
public abstract class KeyboardEventBase<T> : EventBase<T>, IKeyboardEvent where T : KeyboardEventBase<T>, new()
{
	public EventModifiers modifiers { get; protected set; }

	public char character { get; protected set; }

	public KeyCode keyCode { get; protected set; }

	public bool shiftKey => (modifiers & EventModifiers.Shift) != 0;

	public bool ctrlKey => (modifiers & EventModifiers.Control) != 0;

	public bool commandKey => (modifiers & EventModifiers.Command) != 0;

	public bool altKey => (modifiers & EventModifiers.Alt) != 0;

	protected KeyboardEventBase()
	{
		Init();
	}

	protected override void Init()
	{
		base.Init();
		base.flags = EventFlags.Bubbles | EventFlags.Capturable | EventFlags.Cancellable;
		modifiers = EventModifiers.None;
		character = '\0';
		keyCode = KeyCode.None;
	}

	public static T GetPooled(char c, KeyCode keyCode, EventModifiers modifiers)
	{
		T pooled = EventBase<T>.GetPooled();
		pooled.modifiers = modifiers;
		pooled.character = c;
		pooled.keyCode = keyCode;
		return pooled;
	}

	public static T GetPooled(Event systemEvent)
	{
		T pooled = EventBase<T>.GetPooled();
		pooled.imguiEvent = systemEvent;
		if (systemEvent != null)
		{
			pooled.modifiers = systemEvent.modifiers;
			pooled.character = systemEvent.character;
			pooled.keyCode = systemEvent.keyCode;
		}
		return pooled;
	}
}
