using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Gradient used for animating colors.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode]
public sealed class Gradient
{
	internal IntPtr m_Ptr;

	/// <summary>
	///   <para>All color keys defined in the gradient.</para>
	/// </summary>
	public extern GradientColorKey[] colorKeys
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>All alpha keys defined in the gradient.</para>
	/// </summary>
	public extern GradientAlphaKey[] alphaKeys
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Control how the gradient is evaluated.</para>
	/// </summary>
	public extern GradientMode mode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Create a new Gradient object.</para>
	/// </summary>
	[RequiredByNativeCode]
	public Gradient()
	{
		Init();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private extern void Init();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private extern void Cleanup();

	~Gradient()
	{
		Cleanup();
	}

	/// <summary>
	///   <para>Calculate color at a given time.</para>
	/// </summary>
	/// <param name="time">Time of the key (0 - 1).</param>
	public Color Evaluate(float time)
	{
		INTERNAL_CALL_Evaluate(this, time, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Evaluate(Gradient self, float time, out Color value);

	/// <summary>
	///   <para>Setup Gradient with an array of color keys and alpha keys.</para>
	/// </summary>
	/// <param name="colorKeys">Color keys of the gradient (maximum 8 color keys).</param>
	/// <param name="alphaKeys">Alpha keys of the gradient (maximum 8 alpha keys).</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetKeys(GradientColorKey[] colorKeys, GradientAlphaKey[] alphaKeys);
}
