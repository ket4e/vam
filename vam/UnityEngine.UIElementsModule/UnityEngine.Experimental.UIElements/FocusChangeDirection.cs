namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Base class for defining in which direction the focus moves in a focus ring.</para>
/// </summary>
public class FocusChangeDirection
{
	private static readonly FocusChangeDirection s_Unspecified = new FocusChangeDirection(-1);

	private static readonly FocusChangeDirection s_None = new FocusChangeDirection(0);

	private int m_Value;

	/// <summary>
	///   <para>Focus came from an unspecified direction, for example after a mouse down.</para>
	/// </summary>
	public static FocusChangeDirection unspecified => s_Unspecified;

	/// <summary>
	///   <para>The null direction. This is usually used when the focus stays on the same element.</para>
	/// </summary>
	public static FocusChangeDirection none => s_None;

	/// <summary>
	///   <para>Last value for the direction defined by this class.</para>
	/// </summary>
	protected static FocusChangeDirection lastValue => s_None;

	protected FocusChangeDirection(int value)
	{
		m_Value = value;
	}

	public static implicit operator int(FocusChangeDirection fcd)
	{
		return fcd.m_Value;
	}
}
