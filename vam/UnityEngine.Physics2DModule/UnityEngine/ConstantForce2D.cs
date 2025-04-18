using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>Applies both linear and angular (torque) forces continuously to the rigidbody each physics update.</para>
/// </summary>
[NativeHeader("Modules/Physics2D/ConstantForce2D.h")]
[RequireComponent(typeof(Rigidbody2D))]
public sealed class ConstantForce2D : PhysicsUpdateBehaviour2D
{
	/// <summary>
	///   <para>The linear force applied to the rigidbody each physics update.</para>
	/// </summary>
	public Vector2 force
	{
		get
		{
			get_force_Injected(out var ret);
			return ret;
		}
		set
		{
			set_force_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The linear force, relative to the rigid-body coordinate system, applied each physics update.</para>
	/// </summary>
	public Vector2 relativeForce
	{
		get
		{
			get_relativeForce_Injected(out var ret);
			return ret;
		}
		set
		{
			set_relativeForce_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The torque applied to the rigidbody each physics update.</para>
	/// </summary>
	public extern float torque
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_force_Injected(out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_force_Injected(ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_relativeForce_Injected(out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_relativeForce_Injected(ref Vector2 value);
}
