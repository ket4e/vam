namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Define focus change directions for the VisualElementFocusRing.</para>
/// </summary>
public class VisualElementFocusChangeDirection : FocusChangeDirection
{
	private static readonly VisualElementFocusChangeDirection s_Left = new VisualElementFocusChangeDirection((int)FocusChangeDirection.lastValue + 1);

	private static readonly VisualElementFocusChangeDirection s_Right = new VisualElementFocusChangeDirection((int)FocusChangeDirection.lastValue + 2);

	/// <summary>
	///   <para>The focus is moving to the left.</para>
	/// </summary>
	public static FocusChangeDirection left => s_Left;

	/// <summary>
	///   <para>The focus is moving to the right.</para>
	/// </summary>
	public static FocusChangeDirection right => s_Right;

	/// <summary>
	///   <para>Last value for the direction defined by this class.</para>
	/// </summary>
	protected new static VisualElementFocusChangeDirection lastValue => s_Right;

	protected VisualElementFocusChangeDirection(int value)
		: base(value)
	{
	}
}
