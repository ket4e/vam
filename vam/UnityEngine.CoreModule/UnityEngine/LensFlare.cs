using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>Script interface for a.</para>
/// </summary>
[NativeHeader("Runtime/Camera/Flare.h")]
public sealed class LensFlare : Behaviour
{
	/// <summary>
	///   <para>The strength of the flare.</para>
	/// </summary>
	public extern float brightness
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The fade speed of the flare.</para>
	/// </summary>
	public extern float fadeSpeed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The color of the flare.</para>
	/// </summary>
	public Color color
	{
		get
		{
			get_color_Injected(out var ret);
			return ret;
		}
		set
		{
			set_color_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The to use.</para>
	/// </summary>
	public extern Flare flare
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_color_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_color_Injected(ref Color value);
}
