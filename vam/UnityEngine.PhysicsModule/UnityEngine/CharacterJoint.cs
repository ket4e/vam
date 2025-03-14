using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Character Joints are mainly used for Ragdoll effects.</para>
/// </summary>
[NativeClass("Unity::CharacterJoint")]
public sealed class CharacterJoint : Joint
{
	[Obsolete("TargetRotation not in use for Unity 5 and assumed disabled.", true)]
	public Quaternion targetRotation;

	[Obsolete("TargetAngularVelocity not in use for Unity 5 and assumed disabled.", true)]
	public Vector3 targetAngularVelocity;

	[Obsolete("RotationDrive not in use for Unity 5 and assumed disabled.", true)]
	public JointDrive rotationDrive;

	/// <summary>
	///   <para>The secondary axis around which the joint can rotate.</para>
	/// </summary>
	public Vector3 swingAxis
	{
		get
		{
			INTERNAL_get_swingAxis(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_swingAxis(ref value);
		}
	}

	/// <summary>
	///   <para>The configuration of the spring attached to the twist limits of the joint.</para>
	/// </summary>
	public SoftJointLimitSpring twistLimitSpring
	{
		get
		{
			INTERNAL_get_twistLimitSpring(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_twistLimitSpring(ref value);
		}
	}

	/// <summary>
	///   <para>The configuration of the spring attached to the swing limits of the joint.</para>
	/// </summary>
	public SoftJointLimitSpring swingLimitSpring
	{
		get
		{
			INTERNAL_get_swingLimitSpring(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_swingLimitSpring(ref value);
		}
	}

	/// <summary>
	///   <para>The lower limit around the primary axis of the character joint.</para>
	/// </summary>
	public SoftJointLimit lowTwistLimit
	{
		get
		{
			INTERNAL_get_lowTwistLimit(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_lowTwistLimit(ref value);
		}
	}

	/// <summary>
	///   <para>The upper limit around the primary axis of the character joint.</para>
	/// </summary>
	public SoftJointLimit highTwistLimit
	{
		get
		{
			INTERNAL_get_highTwistLimit(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_highTwistLimit(ref value);
		}
	}

	/// <summary>
	///   <para>The angular limit of rotation (in degrees) around the primary axis of the character joint.</para>
	/// </summary>
	public SoftJointLimit swing1Limit
	{
		get
		{
			INTERNAL_get_swing1Limit(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_swing1Limit(ref value);
		}
	}

	/// <summary>
	///   <para>The angular limit of rotation (in degrees) around the primary axis of the character joint.</para>
	/// </summary>
	public SoftJointLimit swing2Limit
	{
		get
		{
			INTERNAL_get_swing2Limit(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_swing2Limit(ref value);
		}
	}

	/// <summary>
	///   <para>Brings violated constraints back into alignment even when the solver fails.</para>
	/// </summary>
	public extern bool enableProjection
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Set the linear tolerance threshold for projection.</para>
	/// </summary>
	public extern float projectionDistance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Set the angular tolerance threshold (in degrees) for projection.</para>
	/// </summary>
	public extern float projectionAngle
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_swingAxis(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_swingAxis(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_twistLimitSpring(out SoftJointLimitSpring value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_twistLimitSpring(ref SoftJointLimitSpring value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_swingLimitSpring(out SoftJointLimitSpring value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_swingLimitSpring(ref SoftJointLimitSpring value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_lowTwistLimit(out SoftJointLimit value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_lowTwistLimit(ref SoftJointLimit value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_highTwistLimit(out SoftJointLimit value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_highTwistLimit(ref SoftJointLimit value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_swing1Limit(out SoftJointLimit value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_swing1Limit(ref SoftJointLimit value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_swing2Limit(out SoftJointLimit value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_swing2Limit(ref SoftJointLimit value);
}
