using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Modules/IMGUI/GUIDebugger.bindings.h")]
internal class GUIDebugger
{
	[NativeConditional("UNITY_EDITOR")]
	public static extern bool active
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[NativeConditional("UNITY_EDITOR")]
	public static void LogLayoutEntry(Rect rect, RectOffset margins, GUIStyle style)
	{
		LogLayoutEntry_Injected(ref rect, margins, style);
	}

	[NativeConditional("UNITY_EDITOR")]
	public static void LogLayoutGroupEntry(Rect rect, RectOffset margins, GUIStyle style, bool isVertical)
	{
		LogLayoutGroupEntry_Injected(ref rect, margins, style, isVertical);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeConditional("UNITY_EDITOR")]
	[StaticAccessor("GetGUIDebuggerManager()", StaticAccessorType.Dot)]
	[NativeMethod("LogEndGroup")]
	public static extern void LogLayoutEndGroup();

	[NativeConditional("UNITY_EDITOR")]
	[StaticAccessor("GetGUIDebuggerManager()", StaticAccessorType.Dot)]
	public static void LogBeginProperty(string targetTypeAssemblyQualifiedName, string path, Rect position)
	{
		LogBeginProperty_Injected(targetTypeAssemblyQualifiedName, path, ref position);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeConditional("UNITY_EDITOR")]
	[StaticAccessor("GetGUIDebuggerManager()", StaticAccessorType.Dot)]
	public static extern void LogEndProperty();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void LogLayoutEntry_Injected(ref Rect rect, RectOffset margins, GUIStyle style);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void LogLayoutGroupEntry_Injected(ref Rect rect, RectOffset margins, GUIStyle style, bool isVertical);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void LogBeginProperty_Injected(string targetTypeAssemblyQualifiedName, string path, ref Rect position);
}
