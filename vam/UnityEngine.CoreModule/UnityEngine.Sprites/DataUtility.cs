using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Sprites;

/// <summary>
///   <para>Helper utilities for accessing Sprite data.</para>
/// </summary>
public sealed class DataUtility
{
	/// <summary>
	///   <para>Inner UV's of the Sprite.</para>
	/// </summary>
	/// <param name="sprite"></param>
	public static Vector4 GetInnerUV(Sprite sprite)
	{
		INTERNAL_CALL_GetInnerUV(sprite, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetInnerUV(Sprite sprite, out Vector4 value);

	/// <summary>
	///   <para>Outer UV's of the Sprite.</para>
	/// </summary>
	/// <param name="sprite"></param>
	public static Vector4 GetOuterUV(Sprite sprite)
	{
		INTERNAL_CALL_GetOuterUV(sprite, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetOuterUV(Sprite sprite, out Vector4 value);

	/// <summary>
	///   <para>Return the padding on the sprite.</para>
	/// </summary>
	/// <param name="sprite"></param>
	public static Vector4 GetPadding(Sprite sprite)
	{
		INTERNAL_CALL_GetPadding(sprite, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetPadding(Sprite sprite, out Vector4 value);

	/// <summary>
	///   <para>Minimum width and height of the Sprite.</para>
	/// </summary>
	/// <param name="sprite"></param>
	public static Vector2 GetMinSize(Sprite sprite)
	{
		Internal_GetMinSize(sprite, out var output);
		return output;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_GetMinSize(Sprite sprite, out Vector2 output);
}
