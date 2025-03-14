namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Event sent just before an element is detach from its parent, if the parent is the descendant of a panel.</para>
/// </summary>
public class DetachFromPanelEvent : EventBase<DetachFromPanelEvent>, IPropagatableEvent
{
	/// <summary>
	///   <para>Constructor. Avoid newing events. Instead, use GetPooled() to get an event from a pool of reusable events.</para>
	/// </summary>
	public DetachFromPanelEvent()
	{
	}
}
