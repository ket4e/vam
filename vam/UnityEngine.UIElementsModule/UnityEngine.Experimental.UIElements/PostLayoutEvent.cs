namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Event sent after the layout is done in a tree. Non-capturable, does not bubble, non-cancellable.</para>
/// </summary>
public class PostLayoutEvent : EventBase<PostLayoutEvent>, IPropagatableEvent
{
	/// <summary>
	///   <para>True if the layout of the element has changed.</para>
	/// </summary>
	public bool hasNewLayout { get; private set; }

	/// <summary>
	///   <para>The old dimensions of the element.</para>
	/// </summary>
	public Rect oldRect { get; private set; }

	/// <summary>
	///   <para>The new dimensions of the element.</para>
	/// </summary>
	public Rect newRect { get; private set; }

	/// <summary>
	///   <para>Constructor. Avoid newing events. Instead, use GetPooled() to get an event from a pool of reusable events.</para>
	/// </summary>
	public PostLayoutEvent()
	{
		Init();
	}

	/// <summary>
	///   <para>Gets an event from the event pool and initializes the event with the specified values. Use this method instead of creating a new event. An event obtained with this method should be released back to the pool using ReleaseEvent().</para>
	/// </summary>
	/// <param name="hasNewLayout">Whether the target layout changed.</param>
	/// <param name="oldRect">The old dimensions of the element.</param>
	/// <param name="newRect">The new dimensions of the element.</param>
	/// <returns>
	///   <para>An event.</para>
	/// </returns>
	public static PostLayoutEvent GetPooled(bool hasNewLayout, Rect oldRect, Rect newRect)
	{
		PostLayoutEvent pooled = EventBase<PostLayoutEvent>.GetPooled();
		pooled.hasNewLayout = hasNewLayout;
		pooled.oldRect = oldRect;
		pooled.newRect = newRect;
		return pooled;
	}

	/// <summary>
	///   <para>Reset the event members to their initial value.</para>
	/// </summary>
	protected override void Init()
	{
		base.Init();
		hasNewLayout = false;
	}
}
