using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Store a collection of Keyframes that can be evaluated over time.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode]
public sealed class AnimationCurve
{
	internal IntPtr m_Ptr;

	/// <summary>
	///   <para>All keys defined in the animation curve.</para>
	/// </summary>
	public Keyframe[] keys
	{
		get
		{
			return GetKeys();
		}
		set
		{
			SetKeys(value);
		}
	}

	public Keyframe this[int index] => GetKey_Internal(index);

	/// <summary>
	///   <para>The number of keys in the curve. (Read Only)</para>
	/// </summary>
	public extern int length
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The behaviour of the animation before the first keyframe.</para>
	/// </summary>
	public extern WrapMode preWrapMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The behaviour of the animation after the last keyframe.</para>
	/// </summary>
	public extern WrapMode postWrapMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Creates an animation curve from an arbitrary number of keyframes.</para>
	/// </summary>
	/// <param name="keys">An array of Keyframes used to define the curve.</param>
	public AnimationCurve(params Keyframe[] keys)
	{
		Init(keys);
	}

	/// <summary>
	///   <para>Creates an empty animation curve.</para>
	/// </summary>
	[RequiredByNativeCode]
	public AnimationCurve()
	{
		Init(null);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private extern void Cleanup();

	~AnimationCurve()
	{
		Cleanup();
	}

	/// <summary>
	///   <para>Evaluate the curve at time.</para>
	/// </summary>
	/// <param name="time">The time within the curve you want to evaluate (the horizontal axis in the curve graph).</param>
	/// <returns>
	///   <para>The value of the curve, at the point in time specified.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern float Evaluate(float time);

	/// <summary>
	///   <para>Add a new key to the curve.</para>
	/// </summary>
	/// <param name="time">The time at which to add the key (horizontal axis in the curve graph).</param>
	/// <param name="value">The value for the key (vertical axis in the curve graph).</param>
	/// <returns>
	///   <para>The index of the added key, or -1 if the key could not be added.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern int AddKey(float time, float value);

	/// <summary>
	///   <para>Add a new key to the curve.</para>
	/// </summary>
	/// <param name="key">The key to add to the curve.</param>
	/// <returns>
	///   <para>The index of the added key, or -1 if the key could not be added.</para>
	/// </returns>
	public int AddKey(Keyframe key)
	{
		return AddKey_Internal(key);
	}

	private int AddKey_Internal(Keyframe key)
	{
		return INTERNAL_CALL_AddKey_Internal(this, ref key);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int INTERNAL_CALL_AddKey_Internal(AnimationCurve self, ref Keyframe key);

	/// <summary>
	///   <para>Removes the keyframe at index and inserts key.</para>
	/// </summary>
	/// <param name="index">The index of the key to move.</param>
	/// <param name="key">The key (with its new time) to insert.</param>
	/// <returns>
	///   <para>The index of the keyframe after moving it.</para>
	/// </returns>
	public int MoveKey(int index, Keyframe key)
	{
		return INTERNAL_CALL_MoveKey(this, index, ref key);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int INTERNAL_CALL_MoveKey(AnimationCurve self, int index, ref Keyframe key);

	/// <summary>
	///   <para>Removes a key.</para>
	/// </summary>
	/// <param name="index">The index of the key to remove.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void RemoveKey(int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void SetKeys(Keyframe[] keys);

	private Keyframe GetKey_Internal(int index)
	{
		INTERNAL_CALL_GetKey_Internal(this, index, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetKey_Internal(AnimationCurve self, int index, out Keyframe value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern Keyframe[] GetKeys();

	/// <summary>
	///   <para>Smooth the in and out tangents of the keyframe at index.</para>
	/// </summary>
	/// <param name="index">The index of the keyframe to be smoothed.</param>
	/// <param name="weight">The smoothing weight to apply to the keyframe's tangents.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SmoothTangents(int index, float weight);

	/// <summary>
	///   <para>Creates a constant "curve" starting at timeStart, ending at timeEnd and with the value value.</para>
	/// </summary>
	/// <param name="timeStart">The start time for the constant curve.</param>
	/// <param name="timeEnd">The start time for the constant curve.</param>
	/// <param name="value">The value for the constant curve.</param>
	/// <returns>
	///   <para>The constant curve created from the specified values.</para>
	/// </returns>
	public static AnimationCurve Constant(float timeStart, float timeEnd, float value)
	{
		return Linear(timeStart, value, timeEnd, value);
	}

	/// <summary>
	///   <para>A straight Line starting at timeStart, valueStart and ending at timeEnd, valueEnd.</para>
	/// </summary>
	/// <param name="timeStart">The start time for the linear curve.</param>
	/// <param name="valueStart">The start value for the linear curve.</param>
	/// <param name="timeEnd">The end time for the linear curve.</param>
	/// <param name="valueEnd">The end value for the linear curve.</param>
	/// <returns>
	///   <para>The linear curve created from the specified values.</para>
	/// </returns>
	public static AnimationCurve Linear(float timeStart, float valueStart, float timeEnd, float valueEnd)
	{
		float num = (valueEnd - valueStart) / (timeEnd - timeStart);
		Keyframe[] array = new Keyframe[2]
		{
			new Keyframe(timeStart, valueStart, 0f, num),
			new Keyframe(timeEnd, valueEnd, num, 0f)
		};
		return new AnimationCurve(array);
	}

	/// <summary>
	///   <para>Creates an ease-in and out curve starting at timeStart, valueStart and ending at timeEnd, valueEnd.</para>
	/// </summary>
	/// <param name="timeStart">The start time for the ease curve.</param>
	/// <param name="valueStart">The start value for the ease curve.</param>
	/// <param name="timeEnd">The end time for the ease curve.</param>
	/// <param name="valueEnd">The end value for the ease curve.</param>
	/// <returns>
	///   <para>The ease-in and out curve generated from the specified values.</para>
	/// </returns>
	public static AnimationCurve EaseInOut(float timeStart, float valueStart, float timeEnd, float valueEnd)
	{
		Keyframe[] array = new Keyframe[2]
		{
			new Keyframe(timeStart, valueStart, 0f, 0f),
			new Keyframe(timeEnd, valueEnd, 0f, 0f)
		};
		return new AnimationCurve(array);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private extern void Init(Keyframe[] keys);
}
