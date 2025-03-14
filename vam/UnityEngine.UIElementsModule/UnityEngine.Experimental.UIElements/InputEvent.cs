namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Sends an event when text from a TextField changes.</para>
/// </summary>
public class InputEvent : EventBase<InputEvent>
{
	/// <summary>
	///   <para>The text before the change occured.</para>
	/// </summary>
	public string previousData { get; protected set; }

	/// <summary>
	///   <para>The new text.</para>
	/// </summary>
	public string newData { get; protected set; }

	/// <summary>
	///   <para>Constructor.</para>
	/// </summary>
	public InputEvent()
	{
		Init();
	}

	/// <summary>
	///   <para>Sets the event to its initial state.</para>
	/// </summary>
	protected override void Init()
	{
		base.Init();
		base.flags = EventFlags.Bubbles | EventFlags.Capturable;
		previousData = null;
		newData = null;
	}

	/// <summary>
	///   <para>Gets an event from the event pool and initializes it with the given values. Use this function instead of newing events. Events obtained from this function should be released back to the pool using ReleaseEvent().</para>
	/// </summary>
	/// <param name="newData">The new text.</param>
	/// <param name="previousData">The text before the change occured.</param>
	/// <returns>
	///   <para>An event.</para>
	/// </returns>
	public static InputEvent GetPooled(string previousData, string newData)
	{
		InputEvent pooled = EventBase<InputEvent>.GetPooled();
		pooled.previousData = previousData;
		pooled.newData = newData;
		return pooled;
	}
}
