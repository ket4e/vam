namespace UnityEngine.Experimental.UIElements;

internal struct KeyboardEventArgs
{
	private readonly EventModifiers m_Modifiers;

	public char character { get; private set; }

	public KeyCode keyCode { get; private set; }

	public bool shift => (m_Modifiers & EventModifiers.Shift) != 0;

	public bool alt => (m_Modifiers & EventModifiers.Alt) != 0;

	public KeyboardEventArgs(char character, KeyCode keyCode, EventModifiers modifiers)
	{
		this = default(KeyboardEventArgs);
		this.character = character;
		this.keyCode = keyCode;
		m_Modifiers = modifiers;
	}

	public Event ToEvent()
	{
		Event @event = new Event();
		@event.character = character;
		@event.keyCode = keyCode;
		@event.modifiers = m_Modifiers;
		@event.type = EventType.KeyDown;
		return @event;
	}
}
