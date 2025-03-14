namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Event sent immediately after an element has gained focus. Capturable, does not bubbles, non-cancellable.</para>
/// </summary>
public class FocusEvent : FocusEventBase<FocusEvent>
{
	/// <summary>
	///   <para>Constructor. Avoid newing events. Instead, use GetPooled() to get an event from a pool of reusable events.</para>
	/// </summary>
	public FocusEvent()
	{
	}
}
