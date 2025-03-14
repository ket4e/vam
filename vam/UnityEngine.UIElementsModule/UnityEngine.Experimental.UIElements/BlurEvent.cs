namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Event sent immediately after an element has lost focus. Capturable, does not bubbles, non-cancellable.</para>
/// </summary>
public class BlurEvent : FocusEventBase<BlurEvent>
{
	/// <summary>
	///   <para>Constructor. Avoid newing events. Instead, use GetPooled() to get an event from a pool of reusable events.</para>
	/// </summary>
	public BlurEvent()
	{
	}
}
