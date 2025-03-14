namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Class used to dispatch IMGUI event types that have no equivalent in UIElements events.</para>
/// </summary>
public class IMGUIEvent : EventBase<IMGUIEvent>
{
	/// <summary>
	///   <para>Constructor. Avoid newing events. Instead, use GetPooled() to get an event from a pool of reusable events.</para>
	/// </summary>
	public IMGUIEvent()
	{
		Init();
	}

	/// <summary>
	///   <para>Get an event from the event pool and initialize it with the given values. Use this function instead of newing events. Events obtained from this function should be released back to the pool using ReleaseEvent().</para>
	/// </summary>
	/// <param name="systemEvent">The IMGUI event used to initialize the event.</param>
	/// <returns>
	///   <para>An event.</para>
	/// </returns>
	public static IMGUIEvent GetPooled(Event systemEvent)
	{
		IMGUIEvent pooled = EventBase<IMGUIEvent>.GetPooled();
		pooled.imguiEvent = systemEvent;
		return pooled;
	}

	/// <summary>
	///   <para>Reset the event members to their initial value.</para>
	/// </summary>
	protected override void Init()
	{
		base.Init();
		base.flags = EventFlags.Bubbles | EventFlags.Capturable | EventFlags.Cancellable;
	}
}
