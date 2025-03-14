namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Event sent after an element is added to an element that is a descendent of a panel.</para>
/// </summary>
public class AttachToPanelEvent : EventBase<AttachToPanelEvent>, IPropagatableEvent
{
	/// <summary>
	///   <para>Constructor. Avoid newing events. Instead, use GetPooled() to get an event from a pool of reusable events.</para>
	/// </summary>
	public AttachToPanelEvent()
	{
	}
}
