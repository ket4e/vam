namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Event sent when the mouse pointer enters an element. Capturable, bubbles, cancellable.</para>
/// </summary>
public class MouseOverEvent : MouseEventBase<MouseOverEvent>
{
	/// <summary>
	///   <para>Constructor. Avoid newing events. Instead, use GetPooled() to get an event from a pool of reusable events.</para>
	/// </summary>
	public MouseOverEvent()
	{
	}
}
