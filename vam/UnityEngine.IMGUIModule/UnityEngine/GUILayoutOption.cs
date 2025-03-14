namespace UnityEngine;

/// <summary>
///   <para>Class internally used to pass layout options into GUILayout functions. You don't use these directly, but construct them with the layouting functions in the GUILayout class.</para>
/// </summary>
public sealed class GUILayoutOption
{
	internal enum Type
	{
		fixedWidth,
		fixedHeight,
		minWidth,
		maxWidth,
		minHeight,
		maxHeight,
		stretchWidth,
		stretchHeight,
		alignStart,
		alignMiddle,
		alignEnd,
		alignJustify,
		equalSize,
		spacing
	}

	internal Type type;

	internal object value;

	internal GUILayoutOption(Type type, object value)
	{
		this.type = type;
		this.value = value;
	}
}
