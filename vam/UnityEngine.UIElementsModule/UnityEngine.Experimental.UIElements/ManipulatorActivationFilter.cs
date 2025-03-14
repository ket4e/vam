namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Used by manipulators to match events against their requirements.</para>
/// </summary>
public struct ManipulatorActivationFilter
{
	/// <summary>
	///   <para>The button that activates the manipulation.</para>
	/// </summary>
	public MouseButton button;

	/// <summary>
	///   <para>Any modifier keys (ie. ctrl, alt, ...) that are needed to activate the manipulation.</para>
	/// </summary>
	public EventModifiers modifiers;

	/// <summary>
	///   <para>Number of mouse clicks required to activate the manipulator.</para>
	/// </summary>
	public int clickCount;

	/// <summary>
	///   <para>Returns true if the current mouse event satisfies the activation requirements.</para>
	/// </summary>
	/// <param name="e">The mouse event.</param>
	/// <returns>
	///   <para>True if the event matches the requirements.</para>
	/// </returns>
	public bool Matches(IMouseEvent e)
	{
		bool flag = clickCount == 0 || e.clickCount >= clickCount;
		return button == (MouseButton)e.button && HasModifiers(e) && flag;
	}

	private bool HasModifiers(IMouseEvent e)
	{
		if (((modifiers & EventModifiers.Alt) != 0 && !e.altKey) || ((modifiers & EventModifiers.Alt) == 0 && e.altKey))
		{
			return false;
		}
		if (((modifiers & EventModifiers.Control) != 0 && !e.ctrlKey) || ((modifiers & EventModifiers.Control) == 0 && e.ctrlKey))
		{
			return false;
		}
		if (((modifiers & EventModifiers.Shift) != 0 && !e.shiftKey) || ((modifiers & EventModifiers.Shift) == 0 && e.shiftKey))
		{
			return false;
		}
		return ((modifiers & EventModifiers.Command) == 0 || e.commandKey) && ((modifiers & EventModifiers.Command) != 0 || !e.commandKey);
	}
}
