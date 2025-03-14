using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>The HingeJoint groups together 2 rigid bodies, constraining them to move like connected by a hinge.</para>
/// </summary>
[NativeClass("Unity::HingeJoint")]
public sealed class HingeJoint : Joint
{
	/// <summary>
	///   <para>The motor will apply a force up to a maximum force to achieve the target velocity in degrees per second.</para>
	/// </summary>
	public JointMotor motor
	{
		get
		{
			INTERNAL_get_motor(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_motor(ref value);
		}
	}

	/// <summary>
	///   <para>Limit of angular rotation (in degrees) on the hinge joint.</para>
	/// </summary>
	public JointLimits limits
	{
		get
		{
			INTERNAL_get_limits(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_limits(ref value);
		}
	}

	/// <summary>
	///   <para>The spring attempts to reach a target angle by adding spring and damping forces.</para>
	/// </summary>
	public JointSpring spring
	{
		get
		{
			INTERNAL_get_spring(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_spring(ref value);
		}
	}

	/// <summary>
	///   <para>Enables the joint's motor. Disabled by default.</para>
	/// </summary>
	public extern bool useMotor
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Enables the joint's limits. Disabled by default.</para>
	/// </summary>
	public extern bool useLimits
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Enables the joint's spring. Disabled by default.</para>
	/// </summary>
	public extern bool useSpring
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The angular velocity of the joint in degrees per second. (Read Only)</para>
	/// </summary>
	public extern float velocity
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The current angle in degrees of the joint relative to its rest position. (Read Only)</para>
	/// </summary>
	public extern float angle
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_motor(out JointMotor value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_motor(ref JointMotor value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_limits(out JointLimits value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_limits(ref JointLimits value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_spring(out JointSpring value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_spring(ref JointSpring value);
}
