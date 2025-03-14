using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>Class for generating random data.</para>
/// </summary>
[NativeHeader("Runtime/Export/Random.bindings.h")]
public sealed class Random
{
	/// <summary>
	///   <para>Serializable structure used to hold the full internal state of the random number generator. See Also: Random.state.</para>
	/// </summary>
	[Serializable]
	public struct State
	{
		[SerializeField]
		private int s0;

		[SerializeField]
		private int s1;

		[SerializeField]
		private int s2;

		[SerializeField]
		private int s3;
	}

	[StaticAccessor("GetScriptingRand()", StaticAccessorType.Dot)]
	[Obsolete("Deprecated. Use InitState() function or Random.state property instead.")]
	public static extern int seed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Gets/Sets the full internal state of the random number generator.</para>
	/// </summary>
	[StaticAccessor("GetScriptingRand()", StaticAccessorType.Dot)]
	public static State state
	{
		get
		{
			get_state_Injected(out var ret);
			return ret;
		}
		set
		{
			set_state_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Returns a random number between 0.0 [inclusive] and 1.0 [inclusive] (Read Only).</para>
	/// </summary>
	public static extern float value
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction]
		get;
	}

	/// <summary>
	///   <para>Returns a random point inside a sphere with radius 1 (Read Only).</para>
	/// </summary>
	public static Vector3 insideUnitSphere
	{
		[FreeFunction]
		get
		{
			get_insideUnitSphere_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>Returns a random point inside a circle with radius 1 (Read Only).</para>
	/// </summary>
	public static Vector2 insideUnitCircle
	{
		get
		{
			GetRandomUnitCircle(out var output);
			return output;
		}
	}

	/// <summary>
	///   <para>Returns a random point on the surface of a sphere with radius 1 (Read Only).</para>
	/// </summary>
	public static Vector3 onUnitSphere
	{
		[FreeFunction]
		get
		{
			get_onUnitSphere_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>Returns a random rotation (Read Only).</para>
	/// </summary>
	public static Quaternion rotation
	{
		[FreeFunction]
		get
		{
			get_rotation_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>Returns a random rotation with uniform distribution (Read Only).</para>
	/// </summary>
	public static Quaternion rotationUniform
	{
		[FreeFunction]
		get
		{
			get_rotationUniform_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>Initializes the random number generator state with a seed.</para>
	/// </summary>
	/// <param name="seed">Seed used to initialize the random number generator.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetScriptingRand()", StaticAccessorType.Dot)]
	[NativeMethod("SetSeed")]
	public static extern void InitState(int seed);

	/// <summary>
	///   <para>Returns a random float number between and min [inclusive] and max [inclusive] (Read Only).</para>
	/// </summary>
	/// <param name="min"></param>
	/// <param name="max"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float Range(float min, float max);

	/// <summary>
	///   <para>Returns a random integer number between min [inclusive] and max [exclusive] (Read Only).</para>
	/// </summary>
	/// <param name="min"></param>
	/// <param name="max"></param>
	public static int Range(int min, int max)
	{
		return RandomRangeInt(min, max);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	private static extern int RandomRangeInt(int min, int max);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	private static extern void GetRandomUnitCircle(out Vector2 output);

	[Obsolete("Use Random.Range instead")]
	public static float RandomRange(float min, float max)
	{
		return Range(min, max);
	}

	[Obsolete("Use Random.Range instead")]
	public static int RandomRange(int min, int max)
	{
		return Range(min, max);
	}

	/// <summary>
	///   <para>Generates a random color from HSV and alpha ranges.</para>
	/// </summary>
	/// <param name="hueMin">Minimum hue [0..1].</param>
	/// <param name="hueMax">Maximum hue [0..1].</param>
	/// <param name="saturationMin">Minimum saturation [0..1].</param>
	/// <param name="saturationMax">Maximum saturation[0..1].</param>
	/// <param name="valueMin">Minimum value [0..1].</param>
	/// <param name="valueMax">Maximum value [0..1].</param>
	/// <param name="alphaMin">Minimum alpha [0..1].</param>
	/// <param name="alphaMax">Maximum alpha [0..1].</param>
	/// <returns>
	///   <para>A random color with HSV and alpha values in the input ranges.</para>
	/// </returns>
	public static Color ColorHSV()
	{
		return ColorHSV(0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f);
	}

	/// <summary>
	///   <para>Generates a random color from HSV and alpha ranges.</para>
	/// </summary>
	/// <param name="hueMin">Minimum hue [0..1].</param>
	/// <param name="hueMax">Maximum hue [0..1].</param>
	/// <param name="saturationMin">Minimum saturation [0..1].</param>
	/// <param name="saturationMax">Maximum saturation[0..1].</param>
	/// <param name="valueMin">Minimum value [0..1].</param>
	/// <param name="valueMax">Maximum value [0..1].</param>
	/// <param name="alphaMin">Minimum alpha [0..1].</param>
	/// <param name="alphaMax">Maximum alpha [0..1].</param>
	/// <returns>
	///   <para>A random color with HSV and alpha values in the input ranges.</para>
	/// </returns>
	public static Color ColorHSV(float hueMin, float hueMax)
	{
		return ColorHSV(hueMin, hueMax, 0f, 1f, 0f, 1f, 1f, 1f);
	}

	/// <summary>
	///   <para>Generates a random color from HSV and alpha ranges.</para>
	/// </summary>
	/// <param name="hueMin">Minimum hue [0..1].</param>
	/// <param name="hueMax">Maximum hue [0..1].</param>
	/// <param name="saturationMin">Minimum saturation [0..1].</param>
	/// <param name="saturationMax">Maximum saturation[0..1].</param>
	/// <param name="valueMin">Minimum value [0..1].</param>
	/// <param name="valueMax">Maximum value [0..1].</param>
	/// <param name="alphaMin">Minimum alpha [0..1].</param>
	/// <param name="alphaMax">Maximum alpha [0..1].</param>
	/// <returns>
	///   <para>A random color with HSV and alpha values in the input ranges.</para>
	/// </returns>
	public static Color ColorHSV(float hueMin, float hueMax, float saturationMin, float saturationMax)
	{
		return ColorHSV(hueMin, hueMax, saturationMin, saturationMax, 0f, 1f, 1f, 1f);
	}

	/// <summary>
	///   <para>Generates a random color from HSV and alpha ranges.</para>
	/// </summary>
	/// <param name="hueMin">Minimum hue [0..1].</param>
	/// <param name="hueMax">Maximum hue [0..1].</param>
	/// <param name="saturationMin">Minimum saturation [0..1].</param>
	/// <param name="saturationMax">Maximum saturation[0..1].</param>
	/// <param name="valueMin">Minimum value [0..1].</param>
	/// <param name="valueMax">Maximum value [0..1].</param>
	/// <param name="alphaMin">Minimum alpha [0..1].</param>
	/// <param name="alphaMax">Maximum alpha [0..1].</param>
	/// <returns>
	///   <para>A random color with HSV and alpha values in the input ranges.</para>
	/// </returns>
	public static Color ColorHSV(float hueMin, float hueMax, float saturationMin, float saturationMax, float valueMin, float valueMax)
	{
		return ColorHSV(hueMin, hueMax, saturationMin, saturationMax, valueMin, valueMax, 1f, 1f);
	}

	/// <summary>
	///   <para>Generates a random color from HSV and alpha ranges.</para>
	/// </summary>
	/// <param name="hueMin">Minimum hue [0..1].</param>
	/// <param name="hueMax">Maximum hue [0..1].</param>
	/// <param name="saturationMin">Minimum saturation [0..1].</param>
	/// <param name="saturationMax">Maximum saturation[0..1].</param>
	/// <param name="valueMin">Minimum value [0..1].</param>
	/// <param name="valueMax">Maximum value [0..1].</param>
	/// <param name="alphaMin">Minimum alpha [0..1].</param>
	/// <param name="alphaMax">Maximum alpha [0..1].</param>
	/// <returns>
	///   <para>A random color with HSV and alpha values in the input ranges.</para>
	/// </returns>
	public static Color ColorHSV(float hueMin, float hueMax, float saturationMin, float saturationMax, float valueMin, float valueMax, float alphaMin, float alphaMax)
	{
		float h = Mathf.Lerp(hueMin, hueMax, value);
		float s = Mathf.Lerp(saturationMin, saturationMax, value);
		float v = Mathf.Lerp(valueMin, valueMax, value);
		Color result = Color.HSVToRGB(h, s, v, hdr: true);
		result.a = Mathf.Lerp(alphaMin, alphaMax, value);
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_state_Injected(out State ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void set_state_Injected(ref State value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_insideUnitSphere_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_onUnitSphere_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_rotation_Injected(out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_rotationUniform_Injected(out Quaternion ret);
}
