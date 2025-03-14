using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Joint is the base class for all joints.</para>
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[NativeClass("Unity::Joint")]
public class Joint : Component
{
	/// <summary>
	///   <para>A reference to another rigidbody this joint connects to.</para>
	/// </summary>
	public extern Rigidbody connectedBody
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The Direction of the axis around which the body is constrained.</para>
	/// </summary>
	public Vector3 axis
	{
		get
		{
			INTERNAL_get_axis(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_axis(ref value);
		}
	}

	/// <summary>
	///   <para>The Position of the anchor around which the joints motion is constrained.</para>
	/// </summary>
	public Vector3 anchor
	{
		get
		{
			INTERNAL_get_anchor(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_anchor(ref value);
		}
	}

	/// <summary>
	///   <para>Position of the anchor relative to the connected Rigidbody.</para>
	/// </summary>
	public Vector3 connectedAnchor
	{
		get
		{
			INTERNAL_get_connectedAnchor(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_connectedAnchor(ref value);
		}
	}

	/// <summary>
	///   <para>Should the connectedAnchor be calculated automatically?</para>
	/// </summary>
	public extern bool autoConfigureConnectedAnchor
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The force that needs to be applied for this joint to break.</para>
	/// </summary>
	public extern float breakForce
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The torque that needs to be applied for this joint to break.</para>
	/// </summary>
	public extern float breakTorque
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Enable collision between bodies connected with the joint.</para>
	/// </summary>
	public extern bool enableCollision
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Toggle preprocessing for this joint.</para>
	/// </summary>
	public extern bool enablePreprocessing
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The force applied by the solver to satisfy all constraints.</para>
	/// </summary>
	public Vector3 currentForce
	{
		get
		{
			INTERNAL_get_currentForce(out var value);
			return value;
		}
	}

	/// <summary>
	///   <para>The torque applied by the solver to satisfy all constraints.</para>
	/// </summary>
	public Vector3 currentTorque
	{
		get
		{
			INTERNAL_get_currentTorque(out var value);
			return value;
		}
	}

	/// <summary>
	///   <para>The scale to apply to the inverse mass and inertia tensor of the body prior to solving the constraints.</para>
	/// </summary>
	public extern float massScale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The scale to apply to the inverse mass and inertia tensor of the connected body prior to solving the constraints.</para>
	/// </summary>
	public extern float connectedMassScale
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
	private extern void INTERNAL_get_axis(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_axis(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_anchor(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_anchor(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_connectedAnchor(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_connectedAnchor(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_currentForce(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_currentTorque(out Vector3 value);
}
