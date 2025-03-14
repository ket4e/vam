namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Sends an event when a value from a field changes.</para>
/// </summary>
public class ChangeEvent<T> : EventBase<ChangeEvent<T>>, IChangeEvent
{
	public T previousValue { get; protected set; }

	public T newValue { get; protected set; }

	public ChangeEvent()
	{
		Init();
	}

	protected override void Init()
	{
		base.Init();
		base.flags = EventFlags.Bubbles | EventFlags.Capturable;
		previousValue = default(T);
		newValue = default(T);
	}

	public static ChangeEvent<T> GetPooled(T previousValue, T newValue)
	{
		ChangeEvent<T> pooled = EventBase<ChangeEvent<T>>.GetPooled();
		pooled.previousValue = previousValue;
		pooled.newValue = newValue;
		return pooled;
	}
}
