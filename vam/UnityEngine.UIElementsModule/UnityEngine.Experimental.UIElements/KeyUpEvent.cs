namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Event sent when a key is released on the keyboard. Capturable, bubbles, cancellable.</para>
/// </summary>
public class KeyUpEvent : KeyboardEventBase<KeyUpEvent>
{
	/// <summary>
	///   <para>Constructor. Avoid newing events. Instead, use GetPooled() to get an event from a pool of reusable events.</para>
	/// </summary>
	public KeyUpEvent()
	{
	}
}
