namespace UnityEngine.Experimental.UIElements.StyleSheets;

internal static class StyleValueUtils
{
	public static bool ApplyAndCompare(ref StyleValue<float> current, StyleValue<float> other)
	{
		float value = current.value;
		if (current.Apply(other, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity))
		{
			return value != other.value;
		}
		return false;
	}

	public static bool ApplyAndCompare(ref StyleValue<int> current, StyleValue<int> other)
	{
		int value = current.value;
		if (current.Apply(other, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity))
		{
			return value != other.value;
		}
		return false;
	}

	public static bool ApplyAndCompare(ref StyleValue<bool> current, StyleValue<bool> other)
	{
		bool value = current.value;
		if (current.Apply(other, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity))
		{
			return value != other.value;
		}
		return false;
	}

	public static bool ApplyAndCompare(ref StyleValue<Color> current, StyleValue<Color> other)
	{
		Color value = current.value;
		if (current.Apply(other, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity))
		{
			return value != other.value;
		}
		return false;
	}

	public static bool ApplyAndCompare(ref StyleValue<CursorStyle> current, StyleValue<CursorStyle> other)
	{
		CursorStyle value = current.value;
		if (current.Apply(other, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity))
		{
			return value != other.value;
		}
		return false;
	}

	public static bool ApplyAndCompare<T>(ref StyleValue<T> current, StyleValue<T> other) where T : Object
	{
		T value = current.value;
		if (current.Apply(other, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity))
		{
			return (Object)value != (Object)other.value;
		}
		return false;
	}
}
