namespace UnityEngine.Experimental.UIElements;

internal struct MouseEventArgs
{
	private readonly EventModifiers m_Modifiers;

	public Vector2 mousePosition { get; private set; }

	public int clickCount { get; private set; }

	public bool shift => (m_Modifiers & EventModifiers.Shift) != 0;

	public MouseEventArgs(Vector2 pos, int clickCount, EventModifiers modifiers)
	{
		this = default(MouseEventArgs);
		mousePosition = pos;
		this.clickCount = clickCount;
		m_Modifiers = modifiers;
	}
}
