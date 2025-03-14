namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Base class for focus related events.</para>
/// </summary>
public abstract class FocusEventBase<T> : EventBase<T>, IFocusEvent, IPropagatableEvent where T : FocusEventBase<T>, new()
{
	public Focusable relatedTarget { get; protected set; }

	public FocusChangeDirection direction { get; protected set; }

	protected FocusEventBase()
	{
		Init();
	}

	protected override void Init()
	{
		base.Init();
		base.flags = EventFlags.Capturable;
		relatedTarget = null;
		direction = FocusChangeDirection.unspecified;
	}

	public static T GetPooled(IEventHandler target, Focusable relatedTarget, FocusChangeDirection direction)
	{
		T pooled = EventBase<T>.GetPooled();
		pooled.target = target;
		pooled.relatedTarget = relatedTarget;
		pooled.direction = direction;
		return pooled;
	}
}
