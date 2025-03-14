using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>A capsule-shaped primitive collider.</para>
/// </summary>
[NativeHeader("Modules/Physics2D/Public/CapsuleCollider2D.h")]
public sealed class CapsuleCollider2D : Collider2D
{
	/// <summary>
	///   <para>The width and height of the capsule area.</para>
	/// </summary>
	public Vector2 size
	{
		get
		{
			get_size_Injected(out var ret);
			return ret;
		}
		set
		{
			set_size_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The direction that the capsule sides can extend.</para>
	/// </summary>
	public extern CapsuleDirection2D direction
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_size_Injected(out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_size_Injected(ref Vector2 value);
}
