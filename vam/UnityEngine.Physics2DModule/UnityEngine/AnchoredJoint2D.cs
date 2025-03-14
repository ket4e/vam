using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>Parent class for all joints that have anchor points.</para>
/// </summary>
[NativeHeader("Modules/Physics2D/AnchoredJoint2D.h")]
public class AnchoredJoint2D : Joint2D
{
	/// <summary>
	///   <para>The joint's anchor point on the object that has the joint component.</para>
	/// </summary>
	public Vector2 anchor
	{
		get
		{
			get_anchor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_anchor_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The joint's anchor point on the second object (ie, the one which doesn't have the joint component).</para>
	/// </summary>
	public Vector2 connectedAnchor
	{
		get
		{
			get_connectedAnchor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_connectedAnchor_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Should the connectedAnchor be calculated automatically?</para>
	/// </summary>
	public extern bool autoConfigureConnectedAnchor
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_anchor_Injected(out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_anchor_Injected(ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_connectedAnchor_Injected(out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_connectedAnchor_Injected(ref Vector2 value);
}
