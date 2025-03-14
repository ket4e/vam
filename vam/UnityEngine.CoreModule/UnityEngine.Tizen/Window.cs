using System;
using UnityEngine.Bindings;

namespace UnityEngine.Tizen;

/// <summary>
///   <para>Interface into Tizen specific functionality.</para>
/// </summary>
[NativeHeader("PlatformDependent/TizenPlayer/TizenBindings.h")]
[NativeConditional("UNITY_TIZEN_API")]
public sealed class Window
{
	/// <summary>
	///   <para>Get pointer to the native window handle.</para>
	/// </summary>
	public unsafe static IntPtr windowHandle => (IntPtr)(void*)null;

	/// <summary>
	///   <para>Get pointer to the Tizen EvasGL object..</para>
	/// </summary>
	public static IntPtr evasGL => IntPtr.Zero;
}
