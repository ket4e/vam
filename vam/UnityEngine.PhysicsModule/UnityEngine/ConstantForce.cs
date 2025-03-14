using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>A force applied constantly.</para>
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public sealed class ConstantForce : Behaviour
{
	/// <summary>
	///   <para>The force applied to the rigidbody every frame.</para>
	/// </summary>
	public Vector3 force
	{
		get
		{
			INTERNAL_get_force(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_force(ref value);
		}
	}

	/// <summary>
	///   <para>The force - relative to the rigid bodies coordinate system - applied every frame.</para>
	/// </summary>
	public Vector3 relativeForce
	{
		get
		{
			INTERNAL_get_relativeForce(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_relativeForce(ref value);
		}
	}

	/// <summary>
	///   <para>The torque applied to the rigidbody every frame.</para>
	/// </summary>
	public Vector3 torque
	{
		get
		{
			INTERNAL_get_torque(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_torque(ref value);
		}
	}

	/// <summary>
	///   <para>The torque - relative to the rigid bodies coordinate system - applied every frame.</para>
	/// </summary>
	public Vector3 relativeTorque
	{
		get
		{
			INTERNAL_get_relativeTorque(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_relativeTorque(ref value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_force(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_force(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_relativeForce(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_relativeForce(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_torque(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_torque(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_relativeTorque(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_relativeTorque(ref Vector3 value);
}
