using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>Asset type that defines the surface properties of a Collider2D.</para>
/// </summary>
[NativeHeader("Modules/Physics2D/Public/PhysicsMaterial2D.h")]
public sealed class PhysicsMaterial2D : Object
{
	/// <summary>
	///   <para>The degree of elasticity during collisions.</para>
	/// </summary>
	public extern float bounciness
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Coefficient of friction.</para>
	/// </summary>
	public extern float friction
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public PhysicsMaterial2D()
	{
		Create_Internal(this, null);
	}

	public PhysicsMaterial2D(string name)
	{
		Create_Internal(this, name);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("Create_Binding")]
	private static extern void Create_Internal([Writable] PhysicsMaterial2D scriptMaterial, string name);
}
