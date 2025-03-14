namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Event sent immediately before an element gains focus. Capturable, bubbles, non-cancellable.</para>
/// </summary>
public class FocusInEvent : FocusEventBase<FocusInEvent>
{
	/// <summary>
	///   <para>Constructor.</para>
	/// </summary>
	public FocusInEvent()
	{
		Init();
	}

	/// <summary>
	///   <para>Reset the event members to their initial value.</para>
	/// </summary>
	protected override void Init()
	{
		base.Init();
		base.flags = EventFlags.Bubbles | EventFlags.Capturable;
	}
}
