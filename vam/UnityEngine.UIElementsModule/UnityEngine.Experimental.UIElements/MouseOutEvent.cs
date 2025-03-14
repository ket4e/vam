namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Event sent when the mouse pointer exits an element. Capturable, bubbles, cancellable.</para>
/// </summary>
public class MouseOutEvent : MouseEventBase<MouseOutEvent>
{
	/// <summary>
	///   <para>Constructor. Avoid newing events. Instead, use GetPooled() to get an event from a pool of reusable events.</para>
	/// </summary>
	public MouseOutEvent()
	{
	}
}
