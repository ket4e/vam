#define UNITY_ASSERTIONS
namespace UnityEngine.Experimental.UIElements.StyleSheets;

/// <summary>
///   <para>This generic structure encodes a value type that can come from USS or be specified programmatically.</para>
/// </summary>
public struct StyleValue<T>
{
	internal int specificity;

	public T value;

	private static readonly StyleValue<T> defaultStyle = default(StyleValue<T>);

	public static StyleValue<T> nil => defaultStyle;

	public StyleValue(T value)
	{
		this.value = value;
		specificity = 0;
	}

	internal StyleValue(T value, int specifity)
	{
		this.value = value;
		specificity = specifity;
	}

	public T GetSpecifiedValueOrDefault(T defaultValue)
	{
		if (specificity > 0)
		{
			defaultValue = value;
		}
		return defaultValue;
	}

	public static implicit operator T(StyleValue<T> sp)
	{
		return sp.value;
	}

	internal bool Apply(StyleValue<T> other, StylePropertyApplyMode mode)
	{
		return Apply(other.value, other.specificity, mode);
	}

	internal bool Apply(T otherValue, int otherSpecificity, StylePropertyApplyMode mode)
	{
		switch (mode)
		{
		case StylePropertyApplyMode.Copy:
			value = otherValue;
			specificity = otherSpecificity;
			return true;
		case StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity:
			if (otherSpecificity >= specificity)
			{
				value = otherValue;
				specificity = otherSpecificity;
				return true;
			}
			return false;
		case StylePropertyApplyMode.CopyIfNotInline:
			if (specificity < int.MaxValue)
			{
				value = otherValue;
				specificity = otherSpecificity;
				return true;
			}
			return false;
		default:
			Debug.Assert(condition: false, "Invalid mode " + mode);
			return false;
		}
	}

	public static implicit operator StyleValue<T>(T value)
	{
		return Create(value);
	}

	public static StyleValue<T> Create(T value)
	{
		return new StyleValue<T>(value, int.MaxValue);
	}

	public override string ToString()
	{
		return string.Format("[StyleProperty<{2}>: specifity={0}, value={1}]", specificity, value, typeof(T).Name);
	}
}
