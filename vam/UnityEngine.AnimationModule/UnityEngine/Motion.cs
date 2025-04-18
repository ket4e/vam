using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>Base class for AnimationClips and BlendTrees.</para>
/// </summary>
[NativeHeader("Runtime/Animation/Motion.h")]
public class Motion : Object
{
	public extern float averageDuration
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public extern float averageAngularSpeed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public Vector3 averageSpeed
	{
		get
		{
			get_averageSpeed_Injected(out var ret);
			return ret;
		}
	}

	public extern float apparentSpeed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public extern bool isLooping
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("IsLooping")]
		get;
	}

	public extern bool legacy
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("IsLegacy")]
		get;
	}

	public extern bool isHumanMotion
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("IsHumanMotion")]
		get;
	}

	[Obsolete("isAnimatorMotion is not supported anymore, please use !legacy instead.", true)]
	public bool isAnimatorMotion { get; }

	protected Motion()
	{
	}

	[Obsolete("ValidateIfRetargetable is not supported anymore, please use isHumanMotion instead.", true)]
	public bool ValidateIfRetargetable(bool val)
	{
		return false;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_averageSpeed_Injected(out Vector3 ret);
}
